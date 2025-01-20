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
            _picker = Property.Children["Picker"];
        }

        protected override void DrawOtherPropertyLayout()
        {
            base.DrawOtherPropertyLayout();
            
            _picker.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_picker, this),
                "调用函数", _picker.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    _picker.Draw(GUIContent.none);
                }
            });
        }
    }
}
