using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_AnimatorTriggerParameterDrawer : AbstractEasyFeedbackDrawer<EF_AnimatorTriggerParameter>
    {
        private InspectorProperty _targetAnimator;
        private InspectorProperty _parameterName;

        protected override void Initialize()
        {
            base.Initialize();
            _targetAnimator = Property.Children["TargetAnimator"];
            _parameterName = Property.Children["ParameterName"];
        }

        protected override void DrawOtherPropertyLayout()
        {
            var value = ValueEntry.SmartValue;

            _targetAnimator.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_targetAnimator, this),
                "触发参数", _targetAnimator.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    if (value.TargetAnimator == null)
                    {
                        EasyEditorGUI.MessageBox("目标动画机不能为空！", MessageType.Error);
                    }

                    _targetAnimator.Draw(new GUIContent("目标动画机"));
                    _parameterName.Draw(new GUIContent("参数名称", "要进行触发的Trigger参数名称"));
                }
            });
        }
    }
}
