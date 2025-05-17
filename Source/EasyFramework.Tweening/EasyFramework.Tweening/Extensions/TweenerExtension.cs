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
        public static T SetUnityObject<T>(this T tweener, UnityEngine.Object unityObject) where T : Tweener
        {
            tweener.UnityObject = unityObject;
            return tweener;
        }

        public static T SetEase<T>(this T tweener, ITweenerEase ease) where T : Tweener
        {
            tweener.Ease = ease;
            return tweener;
        }

        public static T SetEffect<T>(this T tweener, ITweenerEffect effect) where T : Tweener
        {
            tweener.SetEffectWithUpdateProcessor(effect);
            return tweener;
        }

        public static T SetRelative<T>(this T tweener, bool isRelative = true) where T : Tweener
        {
            tweener.IsRelative = isRelative;
            return tweener;
        }

        public static T SetDuration<T>(this T tweener, float duration) where T : Tweener
        {
            tweener.SetDuration(duration);
            return tweener;
        }

        /// <summary>
        /// <para>速度模式，将“持续时间”的值变成“速度”，从“起始值”开始每秒增加“速度”直到“结束值”。</para>
        /// <para>注意：使用此模式后，Tween.Duration()将返回null，除非该Tweener的第一帧被调用了。</para>
        /// </summary>
        public static T SetSpeedBased<T>(this T tweener, bool isSpeedBased = true) where T : Tweener
        {
            tweener.IsSpeedBased = isSpeedBased;
            return tweener;
        }

        public static T SetLoopType<T>(this T tweener, LoopType loopType) where T : Tweener
        {
            tweener.LoopType = loopType;
            return tweener;
        }
    }
}
