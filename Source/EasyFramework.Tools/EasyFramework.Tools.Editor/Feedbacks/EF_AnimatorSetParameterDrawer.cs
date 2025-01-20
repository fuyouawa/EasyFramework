using System;
using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_AnimatorSetParameterDrawer : AbstractEasyFeedbackDrawer<EF_AnimatorSetParameter>
    {
        private InspectorProperty _targetAnimator;
        private InspectorProperty _parameterName;
        private InspectorProperty _parameterType;
        private InspectorProperty _intToSet;
        private InspectorProperty _floatToSet;
        private InspectorProperty _boolToSet;

        protected override void Initialize()
        {
            base.Initialize();
            _targetAnimator = Property.Children["TargetAnimator"];
            _parameterName = Property.Children["ParameterName"];
            _parameterType = Property.Children["ParameterType"];
            _intToSet = Property.Children["IntToSet"];
            _floatToSet = Property.Children["FloatToSet"];
            _boolToSet = Property.Children["BoolToSet"];
        }

        protected override void DrawOtherPropertyLayout()
        {
            var value = ValueEntry.SmartValue;

            _targetAnimator.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_targetAnimator, this),
                "动画控制器参数设置",
                _targetAnimator.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    if (value.TargetAnimator == null)
                    {
                        EasyEditorGUI.MessageBox("目标动画控制器不能为空！", MessageType.Error);
                    }

                    _targetAnimator.Draw(new GUIContent("目标动画控制器"));
                    _parameterName.Draw(new GUIContent("参数名称"));
                    _parameterType.Draw(new GUIContent("参数类型"));

                    switch (value.ParameterType)
                    {
                        case EF_AnimatorSetParameter.ParameterTypes.Int:
                            _intToSet.Draw(new GUIContent("参数设置（Int）"));
                            break;
                        case EF_AnimatorSetParameter.ParameterTypes.Float:
                            _floatToSet.Draw(new GUIContent("参数设置（Float）"));
                            break;
                        case EF_AnimatorSetParameter.ParameterTypes.Bool:
                            _boolToSet.Draw(new GUIContent("参数设置（Bool）"));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            });
        }
    }
}
