namespace EasyFramework.ToolKit
{
    public static class TweenerExtension
    {
        public static AbstractTweener SetEase(this AbstractTweener tweener, IEase ease)
        {
            tweener.Ease = ease;
            return tweener;
        }

        public static AbstractTweener SetEase(this AbstractTweener tweener, ISecondaryEase secondaryEase)
        {
            tweener.SecondaryEase = secondaryEase;
            return tweener;
        }

        public static AbstractTweener SetId(this AbstractTweener tweener, string id)
        {
            tweener.Id = id;
            return tweener;
        }

        public static AbstractTweener SetDuration(this AbstractTweener tweener, float duration)
        {
            tweener.SetDuration(duration);
            return tweener;
        }

        public static AbstractTweener SetDuration(this AbstractTweener tweener, float duration,
            DurationMode durationMode)
        {
            tweener.SetDuration(duration);
            tweener.DurationMode = durationMode;
            return tweener;
        }

        public static AbstractTweener SetDuration(this AbstractTweener tweener, DurationMode durationMode)
        {
            tweener.DurationMode = durationMode;
            return tweener;
        }

        public static AbstractTweener SetLoop(this AbstractTweener tweener, LoopType loopType)
        {
            tweener.LoopType = loopType;
            return tweener;
        }

        public static AbstractTweener SetLoop(this AbstractTweener tweener, int loopCount, LoopType loopType)
        {
            tweener.LoopCount = loopCount;
            tweener.LoopType = loopType;
            return tweener;
        }
    }
}
