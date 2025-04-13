using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace EasyFramework.ToolKit.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class TransformSetPropertiesFeedbackDrawer : AbstractFeedbackDrawer<TransformSetPropertiesFeedback>
    {
        protected override void PostBuildPropertiesGroups()
        {
            PropertiesGroups.Add(new PropertiesGroupInfo("变换设置", rect =>
            {
                if (Feedback.Target == null)
                {
                    EasyEditorGUI.MessageBox("目标不能为空！", MessageType.Error);
                }
                EasyEditorField.UnityObject(
                    EditorHelper.TempContent("目标"),
                    ref Feedback.Target);

                EasyEditorField.Value(
                    EditorHelper.TempContent("修改变换"),
                    ref Feedback.ModifyTransform);

                if (Feedback.ModifyTransform)
                {
                    EasyEditorField.Value(
                        EditorHelper.TempContent("局部空间"),
                        ref Feedback.Local);

                    EasyEditorField.Value(
                        EditorHelper.TempContent("坐标"),
                        ref Feedback.PositionToSet);

                    EasyEditorField.Value(
                        EditorHelper.TempContent("旋转"),
                        ref Feedback.RotationToSet);

                    EasyEditorField.Value(
                        EditorHelper.TempContent("局部缩放"),
                        ref Feedback.LocalScaleToSet);
                }
            }));

            PropertiesGroups.Add(new PropertiesGroupInfo("父级设置", rect =>
            {
                EasyEditorField.Value(
                    EditorHelper.TempContent("修改父级"),
                    ref Feedback.ModifyParent);

                if (Feedback.ModifyParent)
                {
                    EasyEditorField.UnityObject(
                        EditorHelper.TempContent("父级"),
                        ref Feedback.ParentToSet);
                }
            }));
        }
    }
}
