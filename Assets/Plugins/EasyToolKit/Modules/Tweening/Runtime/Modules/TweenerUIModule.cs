using EasyToolKit.Core;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EasyToolKit.Tweening
{
    public static class TweenerUIModule
    {
        public static Tweener DoAnchorPos(this RectTransform target, Vector2 to, float duration)
        {
            return Tween.To(() => target.anchoredPosition, pos => target.anchoredPosition = pos,
                    to, duration)
                .SetUnityObject(target);
        }

        public static Tweener DoAnchorPosX(this RectTransform target, float to, float duration)
        {
            return Tween.To(() => target.anchoredPosition.x,
                    x => target.anchoredPosition = target.anchoredPosition.NewX(x),
                    to, duration)
                .SetUnityObject(target);
        }

        public static Tweener DoAnchorPosY(this RectTransform target, float to, float duration)
        {
            return Tween.To(() => target.anchoredPosition.y,
                    y => target.anchoredPosition = target.anchoredPosition.NewY(y),
                    to, duration)
                .SetUnityObject(target);
        }

        public static Tweener DoFade(this CanvasGroup target, float to, float duration)
        {
            return Tween.To(() => target.alpha, val => target.alpha = val, to, duration)
                .SetUnityObject(target);
        }

        public static Tweener DoMaxVisibleCharacters(this TextMeshProUGUI target, int to, float duration)
        {
            return Tween.To(() => target.maxVisibleCharacters, val => target.maxVisibleCharacters = val, to, duration)
                .SetUnityObject(target);
        }

        public static Tweener DoSpritesAnim(this Image target, Sprite[] sprites, float duration)
        {
            return TweenUtility.PlaySpritesAnimImpl(sprite => target.sprite = sprite, sprites, duration)
                .SetUnityObject(target);
        }
    }
}
