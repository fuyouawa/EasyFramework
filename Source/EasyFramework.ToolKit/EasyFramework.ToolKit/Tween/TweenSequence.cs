using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyFramework.ToolKit
{
    class TweenSequenceNode
    {
        private readonly List<AbstractTween> _tweens = new List<AbstractTween>();
        public TweenSequence Owner { get; }

        public TweenSequenceNode(TweenSequence owner)
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

        public bool IsAllCompleted()
        {
            return !_tweens.Any(tween => tween.CurrentState != TweenState.Completed || tween.LoopCount > 0);
        }

        private bool _isStarted = false;
        private void Start()
        {
            foreach (var tween in _tweens)
            {
                tween.Start();
            }
        }

        public float GetDuration()
        {
            return _tweens.Max(tween => tween.GetActualDuration() ?? 0);
        }

        public void Update()
        {
            if (!_isStarted)
            {
                Start();
                _isStarted = true;
            }

            foreach (var tween in _tweens)
            {
                if (tween.PendingKill)
                    continue;

                tween.Update();

                if (tween.CurrentState == TweenState.Completed)
                {
                    tween.LoopCount--;

                    if (tween.LoopCount > 0)
                    {
                        tween.IsInLoop = true;
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

    public class TweenSequence : AbstractTween
    {
        private readonly List<TweenSequenceNode> _tweenNodes = new List<TweenSequenceNode>();
        private int _currentNodeIndex;

        private float? _actualDuration;
        protected override float? ActualDuration => _actualDuration;

        protected override void OnReset()
        {
            _tweenNodes.Clear();
            _currentNodeIndex = -1;
        }

        public void Append(AbstractTween tween)
        {
            var node = new TweenSequenceNode(this);
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

        protected override void OnStart()
        {
            _currentNodeIndex = 0;
        }

        protected override void OnPlaying(float time)
        {
            if (_currentNodeIndex >= _tweenNodes.Count)
            {
                _actualDuration = 0f;
                foreach (var tweenNode in _tweenNodes)
                {
                    _actualDuration += tweenNode.GetDuration();
                }
                Complete();
                return;
            }

            var node = _tweenNodes[_currentNodeIndex];
            node.Update();

            if (node.IsAllCompleted())
            {
                node.Kill();
                _currentNodeIndex++;
            }
        }
    }
}
