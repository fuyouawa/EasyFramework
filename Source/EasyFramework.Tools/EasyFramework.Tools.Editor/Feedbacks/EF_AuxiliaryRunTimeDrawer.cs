using System;
using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_AuxiliaryRunTimeDrawer : AbstractEasyFeedbackDrawer<EF_AuxiliaryRunTime>
    {
        private InspectorProperty _mode;

        protected override void Initialize()
        {
            base.Initialize();
            _mode = Property.Children["Mode"];
        }

        protected override void DrawOtherPropertyLayout()
        {
            var value = ValueEntry.SmartValue;

            _mode.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_mode, this),
                "运行时间设置", _mode.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    value.Mode = EasyEditorGUI.EnumField(
                        new GUIContent("模式"), value.Mode);

                    switch (value.Mode)
                    {
                        case EF_AuxiliaryRunTime.Modes.Frame:
                            value.Frame = EditorGUILayout.IntField(
                                new GUIContent("帧"), value.Frame);
                            break;
                        case EF_AuxiliaryRunTime.Modes.Seconds:
                            value.Seconds = EditorGUILayout.FloatField(
                                new GUIContent("秒"), value.Seconds);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            });
        }
    }
}
