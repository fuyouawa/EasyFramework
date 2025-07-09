using System;
using UnityEngine;

namespace EasyToolKit.Tweening
{
    internal static class TweenUtility
    {
        public static Tweener PlaySpritesAnimImpl(Action<Sprite> spriteSetter, Sprite[] sprites, float duration)
        {
            int index = 0;
            return Tween.To(() => index, x =>
            {
                index = x;
                spriteSetter(sprites[index]);
            }, sprites.Length - 1, duration);
        }
    }
}
