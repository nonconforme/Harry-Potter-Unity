﻿using System.Collections.Generic;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Game;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    [UsedImplicitly]
    public class Fouled : BaseSpell, IDamageSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            Player.OppositePlayer.TakeDamage(DamageAmount);
            Player.OppositePlayer.OnTurnStartEvent += () => Player.OppositePlayer.AddActions(-1);
        }

        public int DamageAmount { get { return 4; } }
    }
}