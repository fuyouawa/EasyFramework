using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_GameObjectInvokeMethodDrawer : AbstractEasyFeedbackDrawer<EF_GameObjectInvokeMethod>
    {
        private InspectorProperty _picker;

        protected override void Initialize()
        {
            base.Initialize();
            _picker = Property.Children[nameof(EF_GameObjectInvokeMethod.Picker)];
        }
        
        protected override void PostBuildPropertiesGroups()
        {
            PropertiesGroups.Add(new PropertiesGroupInfo("函数调用", rect =>
            {
                _picker.Draw(GUIContent.none);
            }));
        }
    }
}
