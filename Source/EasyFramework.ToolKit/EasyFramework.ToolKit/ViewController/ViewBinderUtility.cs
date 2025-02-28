using System;
using System.Linq;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public static class ViewBinderUtility
    {
        public static Type[] GetBindableComponentTypes(IViewBinder binder)
        {
            var comp = (Component)binder;
            return comp.GetComponents<Component>()
                .Where(c => c != null)
                .Select(c => c.GetType())
                .Distinct()
                .ToArray();
        }

        public static Type[] GetSpecficableBindTypes(Type type)
        {
            return type.GetAllBaseTypes(true, true);
        }
    }
}
