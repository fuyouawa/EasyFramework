using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class MethodPickerParameterDrawer : OdinValueDrawer<MethodPicker.Parameter>
    {
        private InspectorProperty _valueProperty;

        protected override void Initialize()
        {
            base.Initialize();
            _valueProperty = Property.Children[nameof(MethodPicker.Parameter.Value)];
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var val = ValueEntry.SmartValue;
            _valueProperty.Draw(EditorHelper.TempContent(val.Name));
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
