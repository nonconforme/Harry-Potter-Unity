﻿using System.Collections.Generic;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Enums;
using UnityEngine;

namespace HarryPotterUnity.Tween
{
    class FlipCardsTween : ITweenObject
    {
        
        private readonly List<GameObject> _targets;
        private readonly float _time;
        private readonly float _delay;
        private readonly float _timeUntilNextTween;
        private readonly FlipState _flip;
      

        public FlipCardsTween(List<GameObject> targets, FlipState flip, float time = 0.3f, float delay = 0f, float timeUntilNextTween = 0f)
        {
            _targets = targets;
            _flip = flip;
            _time = time;
            _delay = delay;
            _timeUntilNextTween = timeUntilNextTween;
        }


        public float CompletionTime
        {
            get { return _time + _delay; }
        }

        public float TimeUntilNextTween
        {
            get { return _timeUntilNextTween; }
        }

        public void ExecuteTween()
        {
            float targetFlip = 0f;
            switch (_flip)
            {
                case FlipState.FaceUp:
                    targetFlip = 0f;
                    break;
                case FlipState.FaceDown:
                    targetFlip = 180f;
                    break;
            }

            foreach (var target in _targets)
            {
                target.GetComponent<BaseCard>().FlipState = _flip;

                iTween.RotateTo(target, iTween.Hash("time", _time,
                    "delay", _delay,
                    "y", targetFlip,
                    "easetype", iTween.EaseType.EaseInOutSine,
                    "islocal", true
                    ));
            }
            
        }
    }
}
