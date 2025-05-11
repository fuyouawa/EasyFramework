using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyFramework.Tweening
{
    class TweenSequenceClip
    {
        private readonly List<AbstractTween> _tweens = new List<AbstractTween>();
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

            _tweens.Add(tween);
        }

        public bool IsAllPendingKill()
        {
            return _tweens.All(tween => tween.PendingKill);
        }

        public float GetDuration()
        {
            return _tweens.Max(tween => tween.GetActualDuration() ?? 0);
        }

        public void Update()
        {
            foreach (var tween in _tweens)
            {
                if (tween.PendingKill)
                    continue;
                
                if (tween.CurrentState == TweenState.Idle)
                {
                    tween.Start();
                }

                tween.Update();
            }
        }

        public void Kill()
        {
            foreach (var tween in _tweens)
            {
                tween.Kill();
            }
        }
    }

    public class TweenSequence : AbstractTween
    {
        private readonly List<TweenSequenceClip> _tweenClips = new List<TweenSequenceClip>();
        private int _currentNodeIndex;

        private float? _actualDuration;
        protected override float? ActualDuration => _actualDuration;

        protected override void OnReset()
        {
            _tweenClips.Clear();
            _currentNodeIndex = -1;
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
            base.OnStart();
            _currentNodeIndex = 0;
        }

        protected override void OnPlaying(float time)
        {
            if (_currentNodeIndex >= _tweenClips.Count)
            {
                _actualDuration = 0f;
                foreach (var tweenNode in _tweenClips)
                {
                    _actualDuration += tweenNode.GetDuration();
                }
                Complete();
                return;
            }

            var node = _tweenClips[_currentNodeIndex];
            node.Update();

            if (node.IsAllPendingKill())
            {
                node.Kill();
                _currentNodeIndex++;
            }
        }
    }
}
