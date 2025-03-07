using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class ViewBinderConfigDrawer : OdinValueDrawer<ViewBinderConfig>
    {
        private InspectorProperty _property1;
        private InspectorProperty _property2;

        protected override void Initialize()
        {
            _property1 = Property.Children[0];
            _property2 = Property.Children[1];
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            foreach (var component in Property.Components)
            {
                EnsureInitialize(component.Property);
            }

            _property1.Draw(EditorHelper.TempContent("拥有者"));
            _property2.Draw(GUIContent.none);
        }

        private void EnsureInitialize(InspectorProperty property)
        {
        }

        private static Component GetTargetComponent(InspectorProperty property)
        {
            return property.Parent.WeakSmartValue<Component>();
        }
    }
}
