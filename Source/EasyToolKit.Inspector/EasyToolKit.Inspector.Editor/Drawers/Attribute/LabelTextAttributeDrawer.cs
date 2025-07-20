using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Super)]
    public class LabelTextAttributeDrawer : EasyAttributeDrawer<LabelTextAttribute>
    {
        private InspectorText _label;

        protected override void Initialize()
        {
            _label = new InspectorText(Property, Attribute.Label);
        }

        protected override void DrawProperty(GUIContent label)
        {
            label.text = _label.Render();
            CallNextDrawer(label);
        }
    }
}
