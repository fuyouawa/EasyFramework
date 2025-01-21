using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_AnimatorTriggerParameterDrawer : AbstractEasyFeedbackDrawer<EF_AnimatorTriggerParameter>
    {
        protected override void PostBuildPropertiesGroups()
        {
            PropertiesGroups.Add(new PropertiesGroupInfo("参数触发", rect =>
            {
                if (Feedback.TargetAnimator == null)
                {
                    EasyEditorGUI.MessageBox("动画控制器不能为空！", MessageType.Error);
                }

                Feedback.TargetAnimator = EasyEditorField.UnityObject(
                    EditorHelper.TempContent("动画控制器"),
                    Feedback.TargetAnimator);

                Feedback.ParameterName = EasyEditorField.Value(
                    EditorHelper.TempContent("参数名称", "要进行触发的参数名称"),
                    Feedback.ParameterName);
            }));
        }
    }
}
