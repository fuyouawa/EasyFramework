using EasyToolKit.Core.Editor;
using EasyToolKit.ThirdParty.OdinSerializer;
using System;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public static class InspectorPropertyExtensions
    {
        private static readonly GUIContent TempContent = new GUIContent();

        public static void Draw(this InspectorProperty property, string label, string tooltip = null)
        {
            property.Draw(TempContent.SetText(label).SetTooltip(tooltip));
        }

        
        public static LocalPersistentContext<T> GetPersistentContext<T>(this InspectorProperty property, string key, T defaultValue = default)
        {
            var key1 = TwoWaySerializationBinder.Default.BindToName(property.Tree.TargetType);
            var key2 = property.Info.PropertyPath;

            return PersistentContext.GetLocal(string.Join("+", key1, key2, key), defaultValue);
        }
    }
}
