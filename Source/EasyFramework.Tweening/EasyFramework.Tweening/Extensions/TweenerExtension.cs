namespace EasyFramework.Tweening
{
    public static class TweenerExtension
    {
        /// <summary>
        /// 设置所属的unity对象，当对象销毁时也会停止该tweener
        /// </summary>
        /// <param name="tweener"></param>
        /// <param name="unityObject"></param>
        /// <returns></returns>
        public static Tweener SetUnityObject(this Tweener tweener, UnityEngine.Object unityObject)
        {
            tweener.UnityObject = unityObject;
            return tweener;
        }

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

        public static Tweener SetDuration(this Tweener tweener, float duration)
        {
            tweener.SetDuration(duration);
            return tweener;
        }

        /// <summary>
        /// <para>速度模式，将“持续时间”的值变成“速度”，从“起始值”开始每秒增加“速度”直到“结束值”。</para>
        /// <para>注意：使用此模式后，Tween.Duration()将返回null，除非该Tweener的第一帧被调用了。</para>
        /// </summary>
        public static Tweener SetSpeedBased(this Tweener tweener, bool isSpeedBased = true)
        {
            tweener.IsSpeedBased = isSpeedBased;
            return tweener;
        }

        public static Tweener SetLoopType(this Tweener tweener, LoopType loopType)
        {
            tweener.LoopType = loopType;
            return tweener;
        }
    }
}
