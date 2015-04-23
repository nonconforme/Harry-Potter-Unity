﻿using HarryPotterUnity.Tween;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Generic
{
    public abstract class GenericSpell : GenericCard {

        [UsedImplicitly, SerializeField]
        protected Lesson.LessonTypes CostType;

        [UsedImplicitly, SerializeField]
        protected int CostAmount;

        private static readonly Vector3 SpellOffset = new Vector3(0f, 0f, -400f);

        /// <summary>
        /// Describe the actions that happen when this card is played.
        /// For example: Magical Mishap deals 3 damage to the opponent by calling Player.OppositePlayer.TakeDamage(3);
        /// </summary>
        protected abstract void OnPlayAction();

        protected sealed override void OnClickAction()
        {
            AnimateAndDiscard();
            Player.Hand.Remove(this);
        }

        protected override bool MeetsAdditionalPlayRequirements()
        {
            return Player.AmountLessonsInPlay >= CostAmount &&
                   Player.LessonTypesInPlay.Contains(CostType);
        }

        private void AnimateAndDiscard()
        {
            State = CardStates.Discarded;
            var rotate180 = Player.OppositePlayer.IsLocalPlayer ? TweenQueue.RotationType.Rotate180 : TweenQueue.RotationType.NoRotate;
            UtilManager.TweenQueue.AddTweenToQueue(new MoveTween(gameObject, SpellOffset, 0.5f, 0f, !Player.IsLocalPlayer, rotate180, State));
            Invoke("ExecuteActionAndDiscard", 0.9f);
        }

        /// <summary>
        /// Warning: Do not override this when implementing individual cards. 
        /// This method is not sealed because it serves a different purpose in GenericSpellRequiresInput.
        /// </summary>
        [UsedImplicitly]
        protected virtual void ExecuteActionAndDiscard()
        {
            Player.Discard.Add(this);
            OnPlayAction();
            Player.UseActions(ActionCost);
        }
    }
}