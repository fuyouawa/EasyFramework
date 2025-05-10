namespace EasyFramework.ToolKit
{
    public static class TweenerExtension
    {
        public static Tweener SetEase(this Tweener tweener, ITweenerEase ease)
        {
            tweener.Ease = ease;
            return tweener;
        }

        public static Tweener SetEffect(this Tweener tweener, ITweenerEffect effect)
        {
            tweener.SetEffectWithUpdateProcessor(effect);
            return tweener;
        }

        public static Tweener SetRelative(this Tweener tweener, bool isRelative = true)
        {
            tweener.IsRelative = isRelative;
            return tweener;
        }

        public static Tweener SetId(this Tweener tweener, string id)
        {
            tweener.Id = id;
            return tweener;
        }

        public static Tweener SetDuration(this Tweener tweener, float duration)
        {
            tweener.SetDuration(duration);
            return tweener;
        }

        public static Tweener SetDuration(this Tweener tweener, float duration,
            DurationMode durationMode)
        {
            tweener.SetDuration(duration);
            tweener.DurationMode = durationMode;
            return tweener;
        }

        public static Tweener SetDuration(this Tweener tweener, DurationMode durationMode)
        {
            tweener.DurationMode = durationMode;
            return tweener;
        }

        public static Tweener SetLoop(this Tweener tweener, LoopType loopType)
        {
            tweener.LoopType = loopType;
            return tweener;
        }

        public static Tweener SetLoop(this Tweener tweener, int loopCount, LoopType loopType)
        {
            tweener.LoopCount = loopCount;
            tweener.LoopType = loopType;
            return tweener;
        }
    }
}
