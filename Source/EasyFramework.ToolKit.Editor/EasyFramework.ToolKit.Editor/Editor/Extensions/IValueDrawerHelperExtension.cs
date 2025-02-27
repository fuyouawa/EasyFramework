using EasyFramework.ToolKit.Internal;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public static class IValueDrawerHelperExtension
    {
        public static void SetTargetComponent(this IValueDrawerHelper helper, Component component)
        {
            helper.TargetComponent = component;
        }
    }
}
