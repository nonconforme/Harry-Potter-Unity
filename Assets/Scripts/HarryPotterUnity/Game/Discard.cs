﻿using System.Collections.Generic;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    [UsedImplicitly]
    public class Discard : MonoBehaviour
    {
        private List<GenericCard> _cards;

        private static readonly Vector2 DiscardPositionOffset = new Vector2(-355f, -30f);

        //Will use later??
        //public static readonly Vector3 PreviewOffset = new Vector3(-300f, -30f, -6f);

        [UsedImplicitly]
        public void Start () {
            _cards = new List<GenericCard>();

            if (gameObject.GetComponent<Collider>() != null) return;

            var col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(50f, 70f, 1f);
            col.center = new Vector3(DiscardPositionOffset.x, DiscardPositionOffset.y, 0f);
        }

        public void Add(GenericCard card) 
        {
            _cards.Add(card);
            card.transform.parent = transform;

            var cardPos = new Vector3(DiscardPositionOffset.x, DiscardPositionOffset.y, 16f);
            cardPos.z -=  _cards.Count * 0.2f;

            var cardPreviewPos = cardPos;
            cardPreviewPos.z -= 20f;

            var shouldFlip = card.State == GenericCard.CardStates.InDeck;
            var shouldRotate = card.State == GenericCard.CardStates.InPlay;

            UtilManager.TweenQueue.AddTweenToQueue(card, cardPreviewPos, 0.35f, 0f, GenericCard.CardStates.Discarded, shouldFlip, shouldRotate);
            UtilManager.TweenQueue.AddTweenToQueue(card, cardPos, 0.25f, 0f, GenericCard.CardStates.Discarded, false, false);
        }

        //TODO: OnMouseUp: View cards in discard pile
    }
}
