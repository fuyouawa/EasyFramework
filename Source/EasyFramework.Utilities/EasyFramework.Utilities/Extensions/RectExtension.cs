using UnityEngine;

namespace EasyGameFramework
{
    public static class RectExtension
    {
        public static Rect AddX(this Rect rect, float x)
        {
            rect.x += x;
            return rect;
        }

        public static Rect SubX(this Rect rect, float x)
        {
            rect.x -= x;
            return rect;
        }

        public static Rect SetX(this Rect rect, float x)
        {
            rect.x = x;
            return rect;
        }

        public static Rect SubY(this Rect rect, float y)
        {
            rect.y -= y;
            return rect;
        }

        public static Rect AddY(this Rect rect, float y)
        {
            rect.y += y;
            return rect;
        }

        public static Rect SetY(this Rect rect, float y)
        {
            rect.y = y;
            return rect;
        }

        public static Rect SetPosition(this Rect rect, Vector2 position)
        {
            rect.position = position;
            return rect;
        }

        public static Rect SetSize(this Rect rect, Vector2 size)
        {
            rect.size = size;
            return rect;
        }
    }
}
