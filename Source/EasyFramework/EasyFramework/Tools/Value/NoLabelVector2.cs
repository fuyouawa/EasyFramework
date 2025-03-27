using System;
using UnityEngine;

namespace EasyFramework
{
    [Serializable]
    public struct NoLabelVector2
    {
        public float X;
        public float Y;

        public NoLabelVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Vector2(NoLabelVector2 v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static implicit operator NoLabelVector2(Vector2 v)
        {
            return new NoLabelVector2(v.x, v.y);
        }

        public Vector2 ToVec2()
        {
            return new Vector2(X, Y);
        }
    }
}
