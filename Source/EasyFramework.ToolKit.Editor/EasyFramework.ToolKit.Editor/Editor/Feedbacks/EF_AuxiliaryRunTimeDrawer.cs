using System;
using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace EasyFramework.ToolKit.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_AuxiliaryRunTimeDrawer : AbstractEasyFeedbackDrawer<EF_AuxiliaryRunTime>
    {
        protected override void PostBuildPropertiesGroups()
        {
            PropertiesGroups.Add(new PropertiesGroupInfo("时间设置", rect =>
            {
                Feedback.Mode = EasyEditorField.Enum(
                    EditorHelper.TempContent("模式"),
                    Feedback.Mode);

                switch (Feedback.Mode)
                {
                    case EF_AuxiliaryRunTime.Modes.Frame:
                        Feedback.Frame = EditorGUILayout.IntField(
                            EditorHelper.TempContent("帧"),
                            Feedback.Frame);
                        break;
                    case EF_AuxiliaryRunTime.Modes.Seconds:
                        Feedback.Seconds = EditorGUILayout.FloatField(
                            EditorHelper.TempContent("秒"),
                            Feedback.Seconds);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }));
        }
    }
}
