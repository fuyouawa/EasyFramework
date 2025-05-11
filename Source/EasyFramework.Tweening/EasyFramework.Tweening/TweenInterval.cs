using System;

namespace EasyFramework.Tweening
{
    public class TweenInterval : AbstractTween
    {
        private float _duration;
        protected override float? ActualDuration => _duration;

        internal void SetDuration(float duration)
        {
            _duration = duration;
        }

        protected override void OnPlaying(float time)
        {
        }
    }
} 
