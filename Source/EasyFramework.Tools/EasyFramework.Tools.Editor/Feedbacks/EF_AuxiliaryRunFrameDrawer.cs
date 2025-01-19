using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_AuxiliaryRunFrameDrawer : AbstractEasyFeedbackDrawer<EF_AuxiliaryRunFrame>
    {
        private InspectorProperty _frame;

        protected override void Initialize()
        {
            base.Initialize();
            _frame = Property.Children["Frame"];
        }

        protected override void DrawOtherPropertyLayout()
        {
            _frame.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_frame, this),
                "运行指定帧数", _frame.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    _frame.Draw(new GUIContent("帧数"));
                }
            });
        }
    }
}
