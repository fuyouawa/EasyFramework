using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.Editor
{
    public class MethodPickerParameterDrawer : OdinValueDrawer<MethodPicker.Parameter>
    {
        private MethodPicker.Parameter _parameter;
        private InspectorProperty _valueProperty;

        protected override void Initialize()
        {
            base.Initialize();
            _parameter = ValueEntry.SmartValue;
            _valueProperty = Property.Children[nameof(MethodPicker.Parameter.Value)];
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            _valueProperty.Draw(EditorHelper.TempContent(_parameter.Name));
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
