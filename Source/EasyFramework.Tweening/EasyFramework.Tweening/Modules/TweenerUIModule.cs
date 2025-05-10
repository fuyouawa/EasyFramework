using UnityEngine;
using UnityEngine.UI;

namespace EasyFramework.Tweening
{
    public static class TweenerUIModule
    {
        public static Tweener DoArchoredPos(this RectTransform rectTransform, Vector2 endValue, float duration)
        {
            return Tween.To(() => rectTransform.anchoredPosition, pos => rectTransform.anchoredPosition = pos,
                endValue, duration);
        }

        public static Tweener DoSpritesAnim(this Image image, Sprite[] sprites, float duration)
        {
            return Tween.PlaySpritesAnimImpl(sprite => image.sprite = sprite, sprites, duration);
        }
    }
}
