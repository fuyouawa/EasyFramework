using UnityEngine;

namespace EasyGameFramework
{
    public static class ValueExtension
    {
        public static bool Approximately(this float a, float b)
        {
            return Mathf.Approximately(a, b);
        }
    }
}
