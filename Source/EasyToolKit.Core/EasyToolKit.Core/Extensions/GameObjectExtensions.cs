using System;
using UnityEngine;

namespace EasyToolKit.Core
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject target) where T : Component
        {
            if (!target.TryGetComponent(out T component))
            {
                component = target.AddComponent<T>();
            }

            return component;
        }

        public static Component GetOrAddComponent(this GameObject target, Type componentType)
        {
            if (!target.TryGetComponent(componentType, out var component))
            {
                component = target.AddComponent(componentType);
            }

            return component;
        }

        public static bool HasComponent<T>(this GameObject target)
        {
            return target.TryGetComponent<T>(out _);
        }

        public static bool HasComponent(this GameObject target, Type componentType)
        {
            return target.TryGetComponent(componentType, out _);
        }
    }
}
