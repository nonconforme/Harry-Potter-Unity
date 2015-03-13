﻿using System.Collections.Generic;
using HarryPotterUnity.Game;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Generic
{
    [SelectionBase]
    public abstract class GenericCard : MonoBehaviour {

        public enum CardStates
        {
            InDeck, InHand, InPlay, Discarded
        }
        public enum CardTypes
        {
            Lesson, Creature, Spell // Item, Location, Match, Adventure, Character
        }

        public enum Tag
        {
            Healing, Unique
        }

        [UsedImplicitly]
        public List<Tag> Tags;

        public enum ClassificationTypes
        {
            CareOfMagicalCreatures, Charms, Transfiguration, Potions, Quidditch,
            Lesson
           // Character,
           // Adventure
        }

        public enum FlipStates
        {
            FaceUp, FaceDown
        }

        public CardStates State { get; protected set; }
        public FlipStates FlipState { get; private set; }


        [UsedImplicitly, Space(10)] 
        public CardTypes CardType;

        [UsedImplicitly] 
        public ClassificationTypes Classification;

        [UsedImplicitly, SerializeField]
        public int ActionCost = 1;

        public Player Player { get; set; }

        private static readonly Vector2 ColliderSize = new Vector2(50f, 70f);
        private static readonly Vector3 DefaultPreviewCameraPosition = new Vector3(-400, 255, -70);

        private GameObject _cardFace;

        public byte NetworkId { get; set; }

        [UsedImplicitly]
        public void Start()
        {
            FlipState = FlipStates.FaceDown;

            if(gameObject.GetComponent<Collider>() == null)
            {
                var col = gameObject.AddComponent<BoxCollider>();
                col.isTrigger = true;
                col.size = new Vector3(ColliderSize.x, ColliderSize.y, 0.2f);
            }

            gameObject.layer = UtilManager.CardLayer;

            _cardFace = transform.FindChild("Front").gameObject;
        }

        [UsedImplicitly]
        public void SwitchState(CardStates newState)
        {
            State = newState;
        }

        public void SwitchFlipState()
        {
            FlipState = FlipState == FlipStates.FaceUp ? FlipStates.FaceDown : FlipStates.FaceUp;
        }

        [UsedImplicitly]
        public void OnMouseOver()
        {
            //TODO: enable highlight if playable
            ShowPreview();
        }

        [UsedImplicitly]
        public void OnMouseExit()
        {
            HidePreview();
        }

        [UsedImplicitly]
        public void OnMouseUp()
        {
            if (!IsPlayable()) return;
            
            Player.MpGameManager.photonView.RPC("ExecutePlayActionById", PhotonTargets.All, NetworkId);
        }

        private bool IsPlayable()
        {
            return Player.IsLocalPlayer &&
                   State == CardStates.InHand &&
                   Player.CanUseActions(ActionCost) &&
                   MeetsAdditionalPlayRequirements();
        }

        public void MouseUpAction()
        {
            OnClickAction();

            if (CardType != CardTypes.Spell)
            {
                Player.UseActions(ActionCost);
            }
        }

        protected abstract void OnClickAction();
        protected abstract bool MeetsAdditionalPlayRequirements();

        private void ShowPreview()
        {
            _cardFace.layer = UtilManager.PreviewLayer;
            
            if (FlipState == FlipStates.FaceDown) return;

            if (iTween.Count(gameObject) == 0)
            {
                UtilManager.PreviewCamera.transform.rotation = transform.rotation;
                UtilManager.PreviewCamera.transform.position = transform.position + 2 * Vector3.back;
            }
            else
            {
                HidePreview();
            }
        }
    
        private void HidePreview()
        {
            _cardFace.layer = UtilManager.CardLayer;
            UtilManager.PreviewCamera.transform.position = DefaultPreviewCameraPosition;
        }

        public void Disable()
        {
            gameObject.layer = UtilManager.IgnoreRaycastLayer;
            _cardFace.GetComponent<Renderer>().material.color = new Color(0.35f, 0.35f, 0.35f);
        }

        public void Enable()
        {
            gameObject.layer = UtilManager.CardLayer;
            _cardFace.GetComponent<Renderer>().material.color = Color.white;
        }

        public void SetSelected()
        {
            gameObject.layer = UtilManager.CardLayer;
            _cardFace.GetComponent<Renderer>().material.color = Color.yellow;
        }
    }
}
