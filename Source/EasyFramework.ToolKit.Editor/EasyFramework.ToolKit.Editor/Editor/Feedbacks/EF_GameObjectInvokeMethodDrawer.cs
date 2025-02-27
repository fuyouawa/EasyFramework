using EasyFramework.ToolKit;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_GameObjectInvokeMethodDrawer : AbstractEasyFeedbackDrawer<EF_GameObjectInvokeMethod>
    {
        private InspectorProperty _pickerProperty;

        protected override void Initialize()
        {
            base.Initialize();
            _pickerProperty = Property.Children[nameof(EF_GameObjectInvokeMethod.Picker)];
        }
        
        protected override void PostBuildPropertiesGroups()
        {
            PropertiesGroups.Add(new PropertiesGroupInfo("函数调用", rect =>
            {
                _pickerProperty.Draw(GUIContent.none);
            }));
        }
    }
}
