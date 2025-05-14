using System;
using System.Collections.Generic;
using System.Linq;
using EasyFramework.Core;
using UnityEngine;

namespace EasyFramework.Tweening
{
    class TweenSequenceClip
    {
        private readonly List<AbstractTween> _totalTweens = new List<AbstractTween>();
        private readonly RunningTweenList _runningTweens = new RunningTweenList();
        public TweenSequence Owner { get; }

        public TweenSequenceClip(TweenSequence owner)
        {
            Owner = owner;
        }

        public void AddTween(AbstractTween tween)
        {
            if (tween.OwnerSequence != null && tween.OwnerSequence != Owner)
            {
                throw new Exception("Only one sequence can be added to a tween");
            }

            TweenController.Instance.Detach(tween);
            tween.OwnerSequence = Owner;

            _runningTweens.Add(tween);
            _totalTweens.Add(tween);
        }


        public void Kill()
        {
            _runningTweens.Kill();
        }

        public float? GetDuration()
        {
            return _totalTweens.Max(tween => tween.GetActualDuration() ?? 0f);
        }

        public void Update()
        {
            _runningTweens.Update();
        }

        public bool IsAllKilled()
        {
            return _runningTweens.IsAllKilled();
        }
    }

    public class TweenSequence : AbstractTween
    {
        private readonly List<TweenSequenceClip> _tweenClips = new List<TweenSequenceClip>();
        private int _currentClipIndex;

        private float? _actualDuration;
        protected override float? ActualDuration => _actualDuration;

        protected override void OnReset()
        {
            _tweenClips.Clear();
            _currentClipIndex = -1;
        }

        internal void AddTweenAsNewClip(AbstractTween tween)
        {
            var node = new TweenSequenceClip(this);
            node.AddTween(tween);
            _tweenClips.Add(node);
        }

        internal void AddTweenToLastClip(AbstractTween tween)
        {
            var node = _tweenClips.LastOrDefault();
            if (node == null)
            {
                AddTweenAsNewClip(tween);
            }
            else
            {
                node.AddTween(tween);
            }
        }

        protected override void OnStart()
        {
            _currentClipIndex = 0;
        }

        protected override void OnPlaying(float time)
        {
            if (_currentClipIndex >= _tweenClips.Count)
            {
                _actualDuration = 0f;
                foreach (var tweenNode in _tweenClips)
                {
                    _actualDuration += tweenNode.GetDuration();
                }

                Complete();
                return;
            }

            var node = _tweenClips[_currentClipIndex];
            node.Update();

            if (node.IsAllKilled())
            {
                _currentClipIndex++;
            }
        }

        protected override void OnKill()
        {
            var i = _currentClipIndex.Clamp(0, _currentClipIndex);
            for (; i < _tweenClips.Count; i++)
            {
                _tweenClips[i].Kill();
            }
        }
    }
}
