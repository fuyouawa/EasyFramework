using System.Collections.Generic;
using UnityEngine;

namespace EasyFramework.Utilities
{
    public static class RandomUtility
    {
        public static float Range(Vector2 range)
        {
            return Random.Range(range.x, range.y);
        }

        public static T List<T>(IList<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }

        public static T List<T>(T[] list)
        {
            return list[Random.Range(0, list.Length)];
        }

        public static Color Gradient(Gradient gradient)
        {
            return gradient.Evaluate(Random.Range(0f, 1f));
        }
    }
}
