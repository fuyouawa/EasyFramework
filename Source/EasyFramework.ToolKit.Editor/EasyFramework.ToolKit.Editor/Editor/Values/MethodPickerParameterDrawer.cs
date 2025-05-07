using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class MethodPickerParameterDrawer : OdinValueDrawer<MethodPicker.Parameter>
    {
        private InspectorProperty _propertyOfValue;

        protected override void Initialize()
        {
            base.Initialize();
            _propertyOfValue = Property.Children[nameof(MethodPicker.Parameter.Value)];
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var val = ValueEntry.SmartValue;
            _propertyOfValue.Draw(EditorHelper.TempContent(val.Name));
        }
    }

    // public class MethodPickerParameterListDrawer : FoldoutValueDrawer<MethodPicker.ParameterList>
    // {
    //     protected override void OnContentGUI(Rect headerRect)
    //     {
    //         base.OnContentGUI(headerRect);
    //     }
    // }
}
