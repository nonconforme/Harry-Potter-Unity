﻿using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.PlayRequirements;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [RequireComponent(typeof(InputRequirement))]
    public class RavenToWritingDesk : BaseSpell
    {
        public override List<BaseCard> GetFromHandActionTargets()
        {
            return Player.OppositePlayer.InPlay.Creatures;
        }

        protected override bool MeetsAdditionalPlayRequirements()
        {
            return Player.OppositePlayer.InPlay.Creatures.Count >= 2;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var target = targets.First();

            Player.OppositePlayer.Discard.Add(target);
        }
    }
}
