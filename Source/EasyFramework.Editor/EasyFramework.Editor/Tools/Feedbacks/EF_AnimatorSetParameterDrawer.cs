using System;
using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_AnimatorSetParameterDrawer : AbstractEasyFeedbackDrawer<EF_AnimatorSetParameter>
    {
        protected override void PostBuildPropertiesGroups()
        {
            PropertiesGroups.Add(new PropertiesGroupInfo("参数设置", rect =>
            {
                if (Feedback.TargetAnimator == null)
                {
                    EasyEditorGUI.MessageBox("动画控制器不能为空！", MessageType.Error);
                }

                Feedback.TargetAnimator = EasyEditorField.UnityObject(
                    EditorHelper.TempContent("动画控制器"),
                    Feedback.TargetAnimator);

                Feedback.ParameterName = EasyEditorField.Value(
                    EditorHelper.TempContent("参数名称"),
                    Feedback.ParameterName);

                Feedback.ParameterType = EasyEditorField.Enum(
                    EditorHelper.TempContent("参数类型"),
                    Feedback.ParameterType);

                switch (Feedback.ParameterType)
                {
                    case EF_AnimatorSetParameter.ParameterTypes.Int:
                        Feedback.IntToSet = EasyEditorField.Value(
                            EditorHelper.TempContent("参数设置（Int）"),
                            Feedback.IntToSet);
                        break;
                    case EF_AnimatorSetParameter.ParameterTypes.Float:
                        Feedback.FloatToSet = EasyEditorField.Value(
                            EditorHelper.TempContent("参数设置（Float）"),
                            Feedback.FloatToSet);
                        break;
                    case EF_AnimatorSetParameter.ParameterTypes.Bool:
                        Feedback.BoolToSet = EasyEditorField.Value(
                            EditorHelper.TempContent("参数设置（Bool）"),
                            Feedback.BoolToSet);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }));
        }
    }
}
