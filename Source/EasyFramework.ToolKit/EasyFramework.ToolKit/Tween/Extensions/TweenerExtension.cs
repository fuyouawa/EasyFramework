namespace EasyFramework.ToolKit
{
    public static class TweenerExtension
    {
        public static AbstractTweener SetEaseMode(this AbstractTweener tweener, EaseMode easeMode)
        {
            tweener.EaseMode = easeMode;
            return tweener;
        }

        public static AbstractTweener SetId(this AbstractTweener tweener, string id)
        {
            tweener.Id = id;
            return tweener;
        }

        public static AbstractTweener SetDurationMode(this AbstractTweener tweener, DurationMode durationMode)
        {
            tweener.DurationMode = durationMode;
            return tweener;
        }
    }
}
