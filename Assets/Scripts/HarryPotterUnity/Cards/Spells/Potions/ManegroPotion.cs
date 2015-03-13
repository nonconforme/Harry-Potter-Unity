﻿using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;
using LessonType = HarryPotterUnity.Cards.Generic.Lesson.LessonTypes;

namespace HarryPotterUnity.Cards.Spells.Potions
{
    [UsedImplicitly]
    public class ManegroPotion : GenericSpell {

        protected override void OnPlayAction()
        {
            var damage = Player.InPlay.GetAmountOfLessonsOfType(LessonType.Potions);
            var lesson = Player.InPlay.GetLessonOfType(LessonType.Potions);

            Player.Discard.Add(lesson);
            Player.InPlay.Remove(lesson);

            Player.OppositePlayer.TakeDamage(damage);
        }
    }
}
