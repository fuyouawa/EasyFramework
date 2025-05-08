using UnityEngine;
using UnityEngine.UI;

namespace EasyFramework.ToolKit
{
    public static class TweenerUIModule
    {
        public static AbstractTweener MoveArchoredPos(this RectTransform rectTransform, Vector2 endValue, float duration)
        {
            return Tween.To(() => rectTransform.anchoredPosition, pos => rectTransform.anchoredPosition = pos,
                endValue, duration);
        }

        public static AbstractTweener PlaySpritesAnim(this Image image, Sprite[] sprites, float duration)
        {
            return Tween.PlaySpritesAnimImpl(sprite => image.sprite = sprite, sprites, duration);
        }
    }
}
