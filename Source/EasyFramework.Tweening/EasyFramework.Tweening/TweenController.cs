using System.Collections.Generic;
using EasyFramework.Core;

namespace EasyFramework.Tweening
{
    [MonoSingletonConfig(MonoSingletonFlags.DontDestroyOnLoad)]
    public class TweenController : MonoSingleton<TweenController>
    {
        private readonly List<AbstractTween> _runningTweens = new List<AbstractTween>();

        public void Attach(AbstractTween tween)
        {
            _runningTweens.Add(tween);
        }

        public void Detach(AbstractTween tween)
        {
            _runningTweens.Remove(tween);
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
            if (_pendingKillTweens.Count > 0)
            {
                foreach (var tween in _pendingKillTweens)
                {
                    tween.Kill();
                    _runningTweens.Remove(tween);
                }
                _pendingKillTweens.Clear();
            }

            foreach (var tween in _runningTweens)
            {
                if (tween.CurrentState == TweenState.Idle)
                {
                    tween.Start();
                }
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
                        _pendingKillTweens.Add(tween);
                    }
                }
            }
        }
    }
}
