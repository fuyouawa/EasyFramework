using System;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class InlineEditorAttributeDrawer : EasyAttributeDrawer<InlineEditorAttribute>
    {
        protected override bool CanDrawAttributeProperty(InspectorProperty property)
        {
            return property.Children != null;
        }

        protected override void DrawProperty(GUIContent label)
        {
            //TODO InlineEditorAttributeDrawer
        }
    }
}
