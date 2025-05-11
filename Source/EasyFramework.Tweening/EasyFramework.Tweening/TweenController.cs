using System.Collections.Generic;
using EasyFramework.Core;

namespace EasyFramework.Tweening
{
    [MonoSingletonConfig(MonoSingletonFlags.DontDestroyOnLoad)]
    public class TweenController : MonoSingleton<TweenController>
    {
        private readonly List<AbstractTween> _runningTweens = new List<AbstractTween>();
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
                    _tweensById.Remove(id);
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
        
        private readonly List<AbstractTween> _pendingKillTweens = new List<AbstractTween>();

        internal void AddPendingKill(AbstractTween tween)
        {
            Assert.True(_runningTweens.Contains(tween));

            tween.PendingKill = true;
            _pendingKillTweens.Add(tween);
        }

        void Update()
        {
            foreach (var tween in _runningTweens)
            {
                if (tween.PendingKill)
                    continue;

                if (tween.CurrentState == TweenState.Idle)
                {
                    tween.Start();
                }
                tween.Update();
            }

            if (_pendingKillTweens.Count > 0)
            {
                foreach (var tween in _pendingKillTweens)
                {
                    tween.Kill();
                    _runningTweens.Remove(tween);
                }
                _pendingKillTweens.Clear();
            }
        }
    }
}
