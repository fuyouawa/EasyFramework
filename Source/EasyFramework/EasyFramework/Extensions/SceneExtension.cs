using System;
using System.Collections.Generic;
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
            var total = new List<T>();
            foreach (var o in scene.GetRootGameObjects())
            {
                var comps = o.GetComponentsInChildren<T>(includeInactive);
                total.AddRange(comps);
            }

            return total.ToArray();
        }
    }
}
