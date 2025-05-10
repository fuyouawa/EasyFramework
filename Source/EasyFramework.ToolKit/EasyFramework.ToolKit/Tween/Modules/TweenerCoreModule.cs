using UnityEngine;

namespace EasyFramework.ToolKit
{
    public static class TweenerCoreModule
    {
        public static Tweener MoveLocalPos(this Transform transform, Vector3 endPos, float duration)
        {
            return Tween.To(() => transform.localPosition, pos => transform.localPosition = pos, endPos, duration);
        }

        public static Tweener MovePos(this Transform transform, Vector3 endPos, float duration)
        {
            return Tween.To(() => transform.position, pos => transform.position = pos, endPos, duration);
        }

        public static Tweener PlayLocalScale(this Transform transform, Vector3 endLocalScale, float duration)
        {
            return Tween.To(() => transform.localScale, scale => transform.localScale = scale, endLocalScale, duration);
        }

        public static Tweener PlaySpritesAnim(this SpriteRenderer spriteRenderer, Sprite[] sprites,
            float duration)
        {
            return Tween.PlaySpritesAnimImpl(sprite => spriteRenderer.sprite = sprite, sprites, duration);
        }
    }
}
