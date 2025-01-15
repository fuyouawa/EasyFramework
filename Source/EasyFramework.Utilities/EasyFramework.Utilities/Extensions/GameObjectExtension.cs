using UnityEngine;

namespace EasyGameFramework
{
    public static class GameObjectExtension
    {
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            if (!obj.TryGetComponent(out T component))
            {
                component = obj.AddComponent<T>();
            }

            return component;
        }
        public static bool HasComponent<T>(this GameObject go)
            where T : Component
        {
            return go.TryGetComponent<T>(out _);
        }
    }
}
