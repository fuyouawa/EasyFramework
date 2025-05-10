using System;
using UnityEngine;

namespace EasyFramework.Tweening
{
    public static class Tween
    {
        public static Tweener To(Type valueType, TweenGetter getter, TweenSetter setter, object endValue,
            float duration)
        {
            var tweener = TweenManager.Instance.GetTweener();
            tweener.Apply(valueType, getter, setter, endValue);
            tweener.SetDuration(duration);
            return tweener;
        }

        public static Tweener To<T>(TweenGetter<T> getter, TweenSetter<T> setter, T endValue, float duration)
        {
            return To(typeof(T), () => getter(), val => setter((T)val), endValue, duration);
        }

        public static TweenSequence Sequence()
        {
            return TweenManager.Instance.GetSequence();
        }

        public static TweenCallback Callback(Action callback)
        {
            return TweenManager.Instance.GetCallback().AddCallback(callback);
        }

        /// <summary>
        /// <para>获取持续时间，返回null代表此时无法判断具体持续时间。</para>
        /// <para>如果是TweenSequence，只有运行完成了一次，才能确定那一次运行持续的时间，否则将始终返回null。</para>
        /// </summary>
        /// <param name="tween"></param>
        /// <returns></returns>
        public static float? Duration(AbstractTween tween)
        {
            return tween.GetActualDuration();
        }

        public static void Kill(AbstractTween tween)
        {
            tween.PendingKill = true;
            if (tween.OwnerSequence == null)
            {
                TweenController.Instance.AddPendingKill(tween);
            }
        }

        internal static Tweener PlaySpritesAnimImpl(Action<Sprite> spriteSetter, Sprite[] sprites, float duration)
        {
            int index = 0;
            return To(() => index, x =>
            {
                index = x;
                spriteSetter(sprites[index]);
            }, sprites.Length - 1, duration);
        }
    }
}
