using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyFramework.Tweening
{
    internal class RunningTweenList
    {
        private readonly List<AbstractTween> _pendingKillTweens = new List<AbstractTween>();
        private readonly List<AbstractTween> _runningTweens = new List<AbstractTween>();

        public bool IsAllKilled()
        {
            return _runningTweens.All(tween => tween.CurrentState == TweenState.Killed);
        }

        public void Add(AbstractTween tween)
        {
            _runningTweens.Add(tween);
        }

        public void Remove(AbstractTween tween)
        {
            _runningTweens.Remove(tween);
        }

        public void Update()
        {
            if (_runningTweens.Count == 0)
                return;

            foreach (var tween in _runningTweens)
            {
                if (tween.PendingKillSelf)
                {
                    _pendingKillTweens.Add(tween);
                    continue;
                }

                if (tween.CurrentState == TweenState.Idle)
                {
                    try
                    {
                        tween.Start();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to start tweener '{tween}': {e.Message}");
                        tween.PendingKillSelf = true;
                        _pendingKillTweens.Add(tween);
                        continue;
                    }
                }

                tween.Update();
            }

            foreach (var tween in _pendingKillTweens)
            {
                tween.Kill();
                _runningTweens.Remove(tween);
            }

            _pendingKillTweens.Clear();
        }

        public void Kill()
        {
            foreach (var tween in _runningTweens)
            {
                if (tween.CurrentState != TweenState.Killed)
                {
                    tween.Kill();
                }
            }
        }
    }
}
