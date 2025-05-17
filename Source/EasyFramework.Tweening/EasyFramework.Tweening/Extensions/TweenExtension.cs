namespace EasyFramework.Tweening
{
    public static class TweenExtension
    {
        public static T SetId<T>(this T tween, string id) where T : AbstractTween
        {
            tween.Id = id;
            return tween;
        }

        public static T OnKilled<T>(this T tween, TweenEventHandler handler) where T : AbstractTween
        {
            tween.OnKilled += handler;
            return tween;
        }

        public static T OnCompleted<T>(this T tween, TweenEventHandler handler) where T : AbstractTween
        {
            tween.OnCompleted += handler;
            return tween;
        }

        public static T OnPlay<T>(this T tween, TweenEventHandler handler) where T : AbstractTween
        {
            tween.OnPlay += handler;
            return tween;
        }

        public static T OnPause<T>(this T tween, TweenEventHandler handler) where T : AbstractTween
        {
            tween.OnPause += handler;
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
