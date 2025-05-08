using System;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public static class EasyTween
    {
        public static AbstractTweener<T> To<T>(TweenGetter<T> getter, TweenSetter<T> setter, T endValue, float duration)
        {
            var tweener = TweenManager.Instance.GetTweener<T>();
            tweener.Apply(getter, setter, endValue);
            tweener.SetDuration(duration);
            return tweener;
        }

        public static void Kill(AbstractTween tween)
        {
            tween.PendingKill = true;
            if (tween.OwnerSequence == null)
            {
                TweenController.Instance.AddPendingKill(tween);
            }
        }

        internal static AbstractTweener PlaySpritesAnimImpl(Action<Sprite> spriteSetter, Sprite[] sprites, float duration)
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
