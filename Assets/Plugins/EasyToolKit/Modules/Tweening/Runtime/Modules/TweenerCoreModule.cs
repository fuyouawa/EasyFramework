using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Tweening
{
    public static class TweenerCoreModule
    {
        public static Tweener DoLocalMove(this Transform target, Vector3 to, float duration)
        {
            return Tween.To(() => target.localPosition, pos => target.localPosition = pos, to, duration)
                .SetUnityObject(target);
        }

        public static Tweener DoLocalMoveX(this Transform target, float to, float duration)
        {
            return Tween.To(() => target.localPosition.x,
                    x => target.localPosition = target.localPosition.NewX(x),
                    to, duration)
                .SetUnityObject(target);
        }

        public static Tweener DoLocalMoveY(this Transform target, float to, float duration)
        {
            return Tween.To(() => target.localPosition.y,
                    y => target.localPosition = target.localPosition.NewY(y),
                    to, duration)
                .SetUnityObject(target);
        }

        public static Tweener DoLocalMoveZ(this Transform target, float to, float duration)
        {
            return Tween.To(() => target.localPosition.z,
                    z => target.localPosition = target.localPosition.NewZ(z),
                    to, duration)
                .SetUnityObject(target);
        }

        public static Tweener DoMove(this Transform target, Vector3 to, float duration)
        {
            return Tween.To(() => target.position, pos => target.position = pos, to, duration)
                .SetUnityObject(target);
        }

        public static Tweener DoMoveX(this Transform target, float to, float duration)
        {
            return Tween.To(() => target.position.x,
                    x => target.position = target.position.NewX(x),
                    to, duration)
                .SetUnityObject(target);
        }

        public static Tweener DoMoveY(this Transform target, float to, float duration)
        {
            return Tween.To(() => target.position.y,
                    y => target.position = target.position.NewY(y),
                    to, duration)
                .SetUnityObject(target);
        }

        public static Tweener DoMoveZ(this Transform target, float to, float duration)
        {
            return Tween.To(() => target.position.z,
                    z => target.position = target.position.NewZ(z),
                    to, duration)
                .SetUnityObject(target);
        }

        public static Tweener DoScale(this Transform target, Vector3 to, float duration)
        {
            return Tween.To(() => target.localScale, scale => target.localScale = scale, to, duration)
                .SetUnityObject(target);
        }

        public static Tweener DoScale(this Transform target, float to, float duration)
        {
            return target.DoScale(Vector3.one * to, duration);
        }

        public static Tweener DoSpritesAnim(this SpriteRenderer target, Sprite[] sprites,
            float duration)
        {
            return TweenUtility.PlaySpritesAnimImpl(sprite => target.sprite = sprite, sprites, duration)
                .SetUnityObject(target);
        }
    }
}
