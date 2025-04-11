using EasyFramework.ToolKit;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class FeedbackGameObjectInvokeMethodDrawer : AbstractFeedbackDrawer<FeedbackGameObjectInvokeMethod>
    {
        private InspectorProperty _pickerProperty;

        protected override void Initialize()
        {
            base.Initialize();
            _pickerProperty = Property.Children[nameof(FeedbackGameObjectInvokeMethod.Picker)];
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
