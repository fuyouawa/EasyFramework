using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.Core
{
    public static class RandomExtension
    {
        public static float GetRandom(this Vector2 range)
        {
            return Random.Range(range.x, range.y);
        }

        public static T GetRandom<T>(this IList<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }

        public static T GetRandom<T>(this T[] list)
        {
            return list[Random.Range(0, list.Length)];
        }

        public static Color GetRandom(this Gradient gradient)
        {
            return gradient.Evaluate(Random.Range(0f, 1f));
        }
    }
}
