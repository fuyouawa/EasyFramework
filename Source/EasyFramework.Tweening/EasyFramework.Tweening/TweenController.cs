using System;
using System.Collections.Generic;
using EasyFramework.Core;
using UnityEngine;

namespace EasyFramework.Tweening
{
    [MonoSingletonConfig(MonoSingletonFlags.DontDestroyOnLoad)]
    public class TweenController : MonoSingleton<TweenController>
    {
        private readonly RunningTweenList _runningTweens = new RunningTweenList();
        private readonly Dictionary<string, AbstractTween> _tweensById = new Dictionary<string, AbstractTween>();

        public void Attach(AbstractTween tween)
        {
            _runningTweens.Add(tween);
        }

        public void Detach(AbstractTween tween)
        {
            _runningTweens.Remove(tween);
        }

        internal void RegisterTweenById(string id, AbstractTween tween)
        {
            if (id.IsNullOrEmpty())
                return;

            if (_tweensById.TryGetValue(id, out var existingTween))
            {
                if (existingTween != tween)
                {
                    if (!existingTween.IsPendingKill())
                    {
                        throw new ArgumentException($"The id '{id}' has been occupied.");
                    }
                }
                else
                {
                    return;
                }
            }
            _tweensById[id] = tween;
        }

        internal void UnregisterTweenById(string id)
        {
            if (id.IsNullOrEmpty())
                return;

            _tweensById.Remove(id);
        }

        internal AbstractTween GetTweenById(string id)
        {
            if (id.IsNullOrEmpty())
                return null;

            return _tweensById.GetValueOrDefault(id);
        }

        void Update()
        {
            _runningTweens.Update();
        }
    }
}
