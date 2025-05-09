using System;

namespace EasyFramework.ToolKit
{
    public class TweenCallback : AbstractTween
    {
        protected override float? ActualDuration => null;

        internal Action Callback { get; set; }

        protected override void OnStart()
        {
            base.OnStart();
            Callback();
        }

        protected override void OnPlaying(float time)
        {
            Complete();
        }
    }
}
