﻿using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Tween;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    [UsedImplicitly]
    public class Hand : MonoBehaviour {
        
        public List<GenericCard> Cards { get; private set; }

        private Player _player;

        private static readonly Vector3 HandPreviewPosition = new Vector3(-80f, -13f, -336f);

        private static readonly Vector3 HandCardsOffset = new Vector3(-240f, -200f, 0f);

        private const float Spacing = 55f;

        public Hand()
        {
            Cards = new List<GenericCard>();
        }

        [UsedImplicitly]
        public void Awake()
        {
            _player = transform.GetComponentInParent<Player>();
        }

        public void Add(GenericCard card, bool preview = true, bool adjustSpacing = true)
        {
            card.transform.parent = transform;

            var flipState = _player.IsLocalPlayer ? GenericCard.FlipStates.FaceUp : GenericCard.FlipStates.FaceDown;

            AnimateCardToHand(card, flipState, preview);

            if (adjustSpacing) AdjustHandSpacing();

            Cards.Add(card);
        }
        public void AddAll(IEnumerable<GenericCard> cards)
        {
            foreach (var card in cards)
            {
                Add(card, adjustSpacing: false);
            }

            AdjustHandSpacing();
        }

        public void RemoveAll(IEnumerable<GenericCard> cardsToRemove)
        {
            foreach (var card in cardsToRemove)
            {
                Cards.Remove(card);
            }
        }

        public void Remove(GenericCard card)
        {
            RemoveAll(new List<GenericCard> { card });
        }

        public void AdjustHandSpacing()
        {
            ITweenObject tween = new AsyncMoveTween(new List<GenericCard>(Cards), GetTargetPositionForCard);
            UtilManager.TweenQueue.AddTweenToQueue(tween);
        }

        private Vector3 GetTargetPositionForCard(GenericCard card)
        {
            float shrinkFactor = Cards.Count >= 12 ? 0.5f : 1f;
            var cardPosition = HandCardsOffset;

            int index = Cards.IndexOf(card);
            cardPosition.x += index * Spacing * shrinkFactor;
            cardPosition.z -= index;

            return cardPosition;
        }

        private void AnimateCardToHand(GenericCard card, GenericCard.FlipStates flipState, bool preview = true)
        {
            var cardPosition = HandCardsOffset;

            float shrinkFactor = Cards.Count >= 12 ? 0.5f : 1f;

            cardPosition.x += Cards.Count * Spacing * shrinkFactor;
            cardPosition.z -= Cards.Count;

            if (preview)
            {
                UtilManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject, HandPreviewPosition, 0.5f, 0f, flipState, TweenQueue.RotationType.NoRotate, GenericCard.CardStates.InHand));
            }

            UtilManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject, cardPosition, 0.3f, 0.1f, flipState, TweenQueue.RotationType.NoRotate, GenericCard.CardStates.InHand));
        }
    }
}
