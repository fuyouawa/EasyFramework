namespace EasyFramework.ToolKit
{
    public static class TweenExtension
    {
        public static AbstractTween OnKill(this AbstractTween tween, TweenEventHandler handler)
        {
            tween.OnKill += handler;
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

        public static AbstractTween SetLoop(this AbstractTween tween, int loopCount)
        {
            tween.LoopCount = loopCount;
            return tween;
        }
    }
}
