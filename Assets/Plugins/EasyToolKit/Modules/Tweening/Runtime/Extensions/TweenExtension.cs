using System;

namespace EasyToolKit.Tweening
{
    public static class TweenExtension
    {
        public static T SetId<T>(this T tween, string id) where T : AbstractTween
        {
            tween.Id = id;
            return tween;
        }

        public static T OnPlay<T>(this T tween, Action callback) where T : AbstractTween
        {
            tween.AddPlayCallback(_ => callback());
            return tween;
        }

        public static T OnPause<T>(this T tween, Action callback) where T : AbstractTween
        {
            tween.AddPauseCallback(_ => callback());
            return tween;
        }

        public static T OnComplete<T>(this T tween, Action callback) where T : AbstractTween
        {
            tween.AddCompleteCallback(_ => callback());
            return tween;
        }


        public static T OnKill<T>(this T tween, Action callback) where T : AbstractTween
        {
            tween.AddKillCallback(_ => callback());
            return tween;
        }

        public static T SetDelay<T>(this T tween, float delay) where T : AbstractTween
        {
            tween.Delay = delay;
            return tween;
        }

        public static T SetLoopCount<T>(this T tween, int loopCount) where T : AbstractTween
        {
            tween.LoopCount = loopCount;
            return tween;
        }

        public static T SetInfiniteLoop<T>(this T tween, bool infiniteLoop = true) where T : AbstractTween
        {
            tween.InfiniteLoop = infiniteLoop;
            return tween;
        }
    }
}
