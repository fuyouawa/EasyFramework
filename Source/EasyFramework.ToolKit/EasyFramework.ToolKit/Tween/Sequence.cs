using System;
using System.Collections.Generic;
using System.Linq;
using EasyFramework.Core;

namespace EasyFramework.ToolKit
{
    public class Sequence : AbstractTween
    {
        class TweenNode
        {
            private List<AbstractTween> _tweens;
            public float Duration { get; private set; }
            public Sequence Owner { get; }

            public TweenNode(Sequence owner)
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

                if (tween.Duration > Duration)
                {
                    Duration = tween.Duration;
                }
                _tweens.Add(tween);
            }

            public void RefreshDuration()
            {
                Duration = _tweens.Max(tween => tween.Duration);
            }

            public bool IsAllCompleted()
            {
                return !_tweens.Any(tween => tween.CurrentState != TweenState.Completed || tween.Loop > 0);
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

                    if (tween.CurrentState == TweenState.Completed)
                    {
                        tween.Loop--;
                        
                        if (tween.Loop > 0)
                        {
                            tween.Start();
                        }
                        else
                        {
                            tween.PendingKill = true;
                        }
                    }
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
        private readonly List<TweenNode> _tweenNodes = new List<TweenNode>();
        private int _currentNodeIndex;

        private float _duration;

        protected override float GetDuration()
        {
            return _duration;
        }

        protected override void OnReset()
        {
            _tweenNodes.Clear();
            _currentNodeIndex = 0;
            _duration = 0;
        }

        public void Append(AbstractTween tween)
        {
            var node = new TweenNode(this);
            node.AddTween(tween);
            _tweenNodes.Add(node);
        }

        public void Join(AbstractTween tween)
        {
            var node = _tweenNodes.LastOrDefault();
            if (node == null)
            {
                Append(tween);
            }
            else
            {
                node.AddTween(tween);
            }
        }

        internal void RefreshDuration()
        {
            _duration = 0f;
            for (int i = 0; i < _tweenNodes.Count; i++)
            {
                var node = _tweenNodes[i];
                if (i > _currentNodeIndex)
                {
                    node.RefreshDuration();
                }
                _duration += node.Duration;
            }
        }

        protected override void OnPlaying(float time)
        {
            if (_currentNodeIndex >= _tweenNodes.Count)
                return;

            var node = _tweenNodes[_currentNodeIndex];
            node.Update();

            if (node.IsAllCompleted())
            {
                node.Kill();
            }
        }
    }
}
