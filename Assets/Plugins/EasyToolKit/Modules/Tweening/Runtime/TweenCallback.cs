using System;

namespace EasyToolKit.Tweening
{
    public class TweenCallback : AbstractTween
    {
        protected override float? ActualDuration => null;

        internal Action Callback { get; set; }

        protected override void OnStart()
        {
            Callback();
        }

        protected override void OnPlaying(float time)
        {
            Complete();
        }
    }
}
