namespace EasyFramework.Tweening
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

        /// <summary>
        /// <para>速度模式，将“持续时间”的值变成“速度”，从“起始值”开始每秒增加“速度”直到“结束值”。</para>
        /// <para>注意：使用此模式后，Tween.Duration()将返回null，除非该Tweener的第一帧被调用了。</para>
        /// </summary>
        public static Tweener SetSpeedBase(this Tweener tweener, bool isSpeedBase = true)
        {
            tweener.IsSpeedBase = isSpeedBase;
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
