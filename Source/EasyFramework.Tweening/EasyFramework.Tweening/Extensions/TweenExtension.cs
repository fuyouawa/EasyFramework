namespace EasyFramework.Tweening
{
    public static class TweenExtension
    {
        public static AbstractTween SetId(this AbstractTween tween, string id)
        {
            tween.Id = id;
            return tween;
        }

        public static AbstractTween OnKilled(this AbstractTween tween, TweenEventHandler handler)
        {
            tween.OnKilled += handler;
            return tween;
        }

        public static AbstractTween OnCompleted(this AbstractTween tween, TweenEventHandler handler)
        {
            tween.OnCompleted += handler;
            return tween;
        }

        public static AbstractTween OnPlay(this AbstractTween tween, TweenEventHandler handler)
        {
            tween.OnPlay += handler;
            return tween;
        }

        public static AbstractTween OnPause(this AbstractTween tween, TweenEventHandler handler)
        {
            tween.OnPause += handler;
            return tween;
        }

        public static AbstractTween SetDelay(this AbstractTween tween, float delay)
        {
            tween.Delay = delay;
            return tween;
        }

        public static AbstractTween SetLoopCount(this AbstractTween tween, int loopCount)
        {
            tween.LoopCount = loopCount;
            return tween;
        }

        public static AbstractTween SetInfiniteLoop(this AbstractTween tween, bool infiniteLoop = true)
        {
            tween.InfiniteLoop = infiniteLoop;
            return tween;
        }
    }
}
