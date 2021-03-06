﻿using System.Collections.Generic;
using System.Linq;

namespace HarryPotterUnity.Cards.Transfiguration.Items
{
    public class Remembrall : BaseItem
    { 
        public override bool CanPerformInPlayAction()
        {
            return Player.CanUseActions() 
                && Player.Discard.Lessons.Count > 0
                && Player.IsLocalPlayer;
        }

        public override void OnInPlayAction(List<BaseCard> targets = null)
        {
            var lesson = Player.Discard.Lessons.First();

            Player.InPlay.Add(lesson);
            
            Player.UseActions();
        }
    }
}