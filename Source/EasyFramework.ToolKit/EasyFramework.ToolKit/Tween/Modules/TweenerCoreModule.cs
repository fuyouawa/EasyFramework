using UnityEngine;

namespace EasyFramework.ToolKit
{
    public static class TweenerCoreModule
    {
        public static AbstractTweener PlaySpritesAnim(this SpriteRenderer spriteRenderer, Sprite[] sprites,
            float duration)
        {
            return EasyTween.PlaySpritesAnimImpl(sprite => spriteRenderer.sprite = sprite, sprites, duration);
        }
    }
}
