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
        private InspectorProperty _property;

        protected override void Initialize()
        {
            base.Initialize();
            _property = Property.Children["TargetAnimator"];
        }

        protected override void DrawOtherPropertyLayout()
        {
            var value = ValueEntry.SmartValue;

            _property.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_property, this),
                "设置参数",
                _property.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    value.TargetAnimator = EasyEditorGUI.UnityObjectField(
                        new GUIContent("目标动画机"),
                        value.TargetAnimator, true);

                    value.ParameterName = EditorGUILayout.TextField(
                        new GUIContent("参数名称"),
                        value.ParameterName);

                    value.ParameterType = EasyEditorGUI.EnumField(
                        new GUIContent("参数类型"),
                        value.ParameterType);

                    switch (value.ParameterType)
                    {
                        case EF_AnimatorSetParameter.ParameterTypes.Int:
                            value.IntToSet = EditorGUILayout.IntField(
                                new GUIContent("Int To Set"),
                                value.IntToSet);
                            break;
                        case EF_AnimatorSetParameter.ParameterTypes.Float:
                            value.FloatToSet = EditorGUILayout.FloatField(
                                new GUIContent("Float To Set"),
                                value.FloatToSet);
                            break;
                        case EF_AnimatorSetParameter.ParameterTypes.Bool:
                            value.BoolToSet = EditorGUILayout.Toggle(
                                new GUIContent("Bool To Set"),
                                value.BoolToSet);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            });
        }
    }
}
