﻿using System.Collections.Generic;
using System.Linq;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    public class NoHands : BaseSpell
    {
        protected override bool MeetsAdditionalPlayRequirements()
        {
            return Player.InPlay.Matches.Any() || Player.OppositePlayer.InPlay.Matches.Any();
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var topCard = Player.Deck.TakeTopCard();

            Player.Discard.Add(topCard);

            if (topCard is BaseLesson)
            {
                var match = (BaseMatch) Player.InPlay.Matches.Concat(Player.OppositePlayer.InPlay.Matches).Single();

                Player.Discard.Add(match);
                match.OnPlayerHasWonMatch(Player, Player.OppositePlayer);
            }
            else
            {
                Player.TakeDamage(this, 3);
            }
            
        }
    }
}