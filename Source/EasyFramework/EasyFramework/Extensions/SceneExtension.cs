using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EasyFramework
{
    public static class SceneExtension
    {
        public static Component[] FindObjectsByType(this Scene scene, Type type, bool includeInactive = false)
        {
            var total = new List<Component>();
            foreach (var o in scene.GetRootGameObjects())
            {
                var comps = o.GetComponentsInChildren(type, includeInactive);
                total.AddRange(comps);
            }

            return total.ToArray();
        }

        public static T[] FindObjectsByType<T>(this Scene scene, bool includeInactive = false)
        {
            return scene.FindObjectsByType(typeof(T), includeInactive)
                .Select(c => (T)(object)c)
                .ToArray();
        }

        public static Component FindFirstObjectByType(this Scene scene, Type type, bool includeInactive = false)
        {
            foreach (var o in scene.GetRootGameObjects())
            {
                var comp = o.GetComponentInChildren(type, includeInactive);
                if (comp != null)
                {
                    return comp;
                }
            }
            return null;
        }

        public static T FindFirstObjectByType<T>(this Scene scene, bool includeInactive = false)
        {
            return (T)(object)scene.FindFirstObjectByType(typeof(T), includeInactive);
        }
    }
}
