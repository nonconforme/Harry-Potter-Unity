﻿using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Tween;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    [UsedImplicitly]
    public class Discard : CardCollection
    {
        private static readonly Vector2 DiscardPositionOffset = new Vector2(-355f, -30f);

        [UsedImplicitly]
        public void Start () {
            Cards = new List<BaseCard>();
        }

        public override void Add(BaseCard card) 
        {
            Cards.Add(card);
            card.Enable();
            
            card.transform.parent = transform;

            var cardPos = new Vector3(DiscardPositionOffset.x, DiscardPositionOffset.y, 16f);
            cardPos.z -=  Cards.Count * 0.2f;

            var cardPreviewPos = cardPos;
            cardPreviewPos.z -= 20f;
            
            GameManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject, cardPreviewPos, 0.35f, 0f, FlipStates.FaceUp, RotationType.NoRotate, State.Discarded));
            GameManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject, cardPos, 0.25f, 0f, FlipStates.FaceUp, RotationType.NoRotate, State.Discarded));

            MoveToThisCollection(card);
        }

        protected override void Remove(BaseCard card)
        {
            Cards.Remove(card);
        }

        public override void AddAll(IEnumerable<BaseCard> cards)
        {

            var cardList = cards as List<BaseCard> ?? cards.ToList();

            foreach (var card in cardList)
            {
                Add(card);
            }


            AdjustCardSpacing();
        }

        public override void RemoveAll(IEnumerable<BaseCard> cards)
        {
            foreach (var card in cards)
            {
                Cards.Remove(card);
            }
        }
        
        public int CountCards(Func<BaseCard, bool> predicate)
        {
            return Cards.Count(predicate);
        }

        private void AdjustCardSpacing()
        {
            ITweenObject tween = new AsyncMoveTween(new List<BaseCard>(Cards), GetTargetPositionForCard);
            GameManager.TweenQueue.AddTweenToQueue(tween);
        }

        private Vector3 GetTargetPositionForCard(BaseCard card)
        {
            if (!Cards.Contains(card)) return card.transform.localPosition;

            int position = Cards.IndexOf(card);

            var cardPos = new Vector3(DiscardPositionOffset.x, DiscardPositionOffset.y, 16f);
            cardPos.z -= position * 0.2f;
            
            return cardPos;
        }

        public List<BaseCard> GetHealableCards(int amount)
        {
            return Cards.Where(card => !card.Tags.Contains(Tag.Healing)).Take(amount).ToList(); 
        }
    }
}
