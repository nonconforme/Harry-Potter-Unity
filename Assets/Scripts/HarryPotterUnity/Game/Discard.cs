﻿using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Tween;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    [UsedImplicitly]
    public class Discard : MonoBehaviour
    {
        private List<BaseCard> _cards;
        
        private static readonly Vector2 DiscardPositionOffset = new Vector2(-355f, -30f);

        [UsedImplicitly]
        public void Start () {
            _cards = new List<BaseCard>();

            /*
            if (gameObject.GetComponent<Collider>() != null) return;
            
            var col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(50f, 70f, 1f);
            col.center = new Vector3(DiscardPositionOffset.x, DiscardPositionOffset.y, 0f);
             * */
        }

        public void Add(BaseCard card) 
        {
            _cards.Add(card);
            card.Enable();

            card.transform.parent = transform;

            var cardPos = new Vector3(DiscardPositionOffset.x, DiscardPositionOffset.y, 16f);
            cardPos.z -=  _cards.Count * 0.2f;

            var cardPreviewPos = cardPos;
            cardPreviewPos.z -= 20f;

            GameManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject, cardPreviewPos, 0.35f, 0f, FlipStates.FaceUp, TweenQueue.RotationType.NoRotate, State.Discarded));
            GameManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject, cardPos, 0.25f, 0f, FlipStates.FaceUp, TweenQueue.RotationType.NoRotate, State.Discarded));
        }

        public void AddAll(IEnumerable<BaseCard> cards)
        {
            AdjustCardSpacing();

            foreach (var card in cards)
            {
                Add(card);
            }
        }

        public void RemoveAll(IEnumerable<BaseCard> cards)
        {
            foreach (var card in cards)
            {
                _cards.Remove(card);
            }
        }

        public List<BaseCard> GetCards(Predicate<BaseCard> predicate)
        {
            return _cards.FindAll(predicate).ToList();
        }

        public int CountCards(Func<BaseCard, bool> predicate)
        {
            return _cards.Count(predicate);
        }

        private void AdjustCardSpacing()
        {
            ITweenObject tween = new AsyncMoveTween(new List<BaseCard>(_cards), GetTargetPositionForCard);
            GameManager.TweenQueue.AddTweenToQueue(tween);
        }

        private Vector3 GetTargetPositionForCard(BaseCard card)
        {
            int position = _cards.IndexOf(card);

            var cardPos = new Vector3(DiscardPositionOffset.x, DiscardPositionOffset.y, 16f);
            cardPos.z -= position * 0.2f;
            
            return cardPos;
        }
    }
}
