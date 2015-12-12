﻿using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;
using Type = HarryPotterUnity.Enums.Type;

namespace HarryPotterUnity.DeckGeneration
{
    [UsedImplicitly]
    public static class DeckGenerator
    {
        private static List<BaseCard> _cardLibrary;

        private static List<BaseCard> CardLibrary
        {
            get {
                    if (_cardLibrary == null) LoadCardLibrary();
                    return _cardLibrary;
                }
        }

        private static List<BaseCard> _availableStartingCharacters;
        private static List<BaseCard> _allStartingCharacters;

        private static void LoadCardLibrary()
        {
            _cardLibrary = new List<BaseCard>();
            _allStartingCharacters = new List<BaseCard>();
            _availableStartingCharacters = new List<BaseCard>();

            var resources = Resources.LoadAll("Cards/");
            
            foreach (GameObject container in resources.Cast<GameObject>())
            {
                if (container == null)
                {
                    Debug.LogError("Failed to load asset");
                    continue;
                }
                var cardInfo = container.GetComponent<BaseCard>();
                _cardLibrary.Add(cardInfo);

                if (cardInfo.Type == Type.Character)
                {
                    _availableStartingCharacters.Add(cardInfo);
                    _allStartingCharacters.Add(cardInfo);
                }
            }
        }

        public static BaseCard GetRandomStartingCharacter()
        {
            if(_availableStartingCharacters == null) LoadCardLibrary();

            if (_availableStartingCharacters == null)
            {
                throw new Exception("Starting Characters are not loaded!");
            }

            BaseCard character = _availableStartingCharacters.Skip(Random.Range(0, _availableStartingCharacters.Count)).First();

            _availableStartingCharacters.Remove(character);

            return character;
        }

        public static IEnumerable<BaseCard> GenerateDeck(List<LessonTypes> types)
        {
            var deck = new List<BaseCard>();

            switch (types.Count)
            {
                case 2:
                    AddLessonsToDeck(ref deck, types[0], 16);
                    AddLessonsToDeck(ref deck, types[1], 14);

                    AddCardsToDeck(ref deck, MapLessonType(types[0]), 15);
                    AddCardsToDeck(ref deck, MapLessonType(types[1]), 15);
                    break;
                case 3:
                    AddLessonsToDeck(ref deck, types[0], 15);
                    AddLessonsToDeck(ref deck, types[1], 8);
                    AddLessonsToDeck(ref deck, types[2], 7);

                    AddCardsToDeck(ref deck, MapLessonType(types[0]), 10);
                    AddCardsToDeck(ref deck, MapLessonType(types[1]), 10);                   
                    AddCardsToDeck(ref deck, MapLessonType(types[2]), 10);
                    break;
                default:
                    throw new Exception(types.Count + " type(s) sent to GenerateDeck, unsupported");
            }

            return deck;
        }

        private static ClassificationTypes MapLessonType(LessonTypes type)
        {
            switch (type)
            {
                    case LessonTypes.Creatures: return ClassificationTypes.CareOfMagicalCreatures;
                    case LessonTypes.Charms: return ClassificationTypes.Charms;
                    case LessonTypes.Transfiguration: return  ClassificationTypes.Transfiguration;
                    case LessonTypes.Quidditch: return  ClassificationTypes.Quidditch;
                    case LessonTypes.Potions: return ClassificationTypes.Potions;
                default:
                    throw new ArgumentException("Unable to map lesson type");
            }
        }

        private static void AddLessonsToDeck(ref List<BaseCard> deck, LessonTypes lessonType, int amount)
        {
            BaseCard card = CardLibrary.Where(c => c.Classification == ClassificationTypes.Lesson).First(l => ((ILessonProvider) l).LessonType == lessonType);

            for (int i = 0; i < amount; i++)
            {
                deck.Add(card);
            }
        }

        private static void AddCardsToDeck(ref List<BaseCard> deck, ClassificationTypes classification, int amount)
        {
            var potentialCards = CardLibrary.Where(c => c.Classification == classification).ToList();

            int cardsAdded = 0;

            while (cardsAdded < amount)
            {
                int selected = Random.Range(0, potentialCards.Count);
                var card = potentialCards[selected];

                var deckCopy = deck.ToList();

                bool canBeAdded = (card.DeckGenerationRequirements.Count == 0 || card.DeckGenerationRequirements.TrueForAll(req => req.MeetsRequirement(deckCopy))) && card.MeetsRarityRequirements();

                //TODO: Enabled the second check when enough cards have been implemented
                if (canBeAdded == false /* || deck.Count(c => c.Equals(card)) >= 4 */) continue;

                deck.Add(card);
                cardsAdded++;
            }
        }

        private static bool MeetsRarityRequirements(this BaseCard card)
        {
            float chanceToAdd;

            float rng = Random.Range(0f, 1f);

            switch (card.Rarity)
            {
                case Rarity.Common:
                    chanceToAdd = 1f;
                    break;
                case Rarity.Uncommon:
                    chanceToAdd = 0.7f;
                    break;
                case Rarity.Rare:
                    chanceToAdd = 0.5f;
                    break;
                case Rarity.UltraRare:
                    chanceToAdd = 0.3f;
                    break;
                default:
                    chanceToAdd = 1f;
                    break;
            }

            return rng <= chanceToAdd;
        }

        public static void ResetStartingCharacterPool()
        {
            if (_allStartingCharacters == null)
            {
                LoadCardLibrary();
            }
            if (_allStartingCharacters != null)
            {
                _availableStartingCharacters = _allStartingCharacters.ToList();
            }
            
        }
    }
}