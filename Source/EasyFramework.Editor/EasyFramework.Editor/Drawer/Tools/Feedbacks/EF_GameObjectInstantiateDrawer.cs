using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using System;
using UnityEditor;

namespace EasyFramework.Editor.Drawer
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_GameObjectInstantiateDrawer : AbstractEasyFeedbackDrawer<EF_GameObjectInstantiate>
    {
        protected override void PostBuildPropertiesGroups()
        {
            PropertiesGroups.Add(new PropertiesGroupInfo("预制体设置", rect =>
            {
                if (Feedback.Prefab == null)
                {
                    EasyEditorGUI.MessageBox("预制体不能为空！", MessageType.Error);
                }
                    
                Feedback.Prefab = EasyEditorField.UnityObject(
                    EditorHelper.TempContent("预制体"),
                    Feedback.Prefab);

                Feedback.HasLiftTime = EasyEditorField.Value(
                    EditorHelper.TempContent("拥有生命时间"),
                    Feedback.HasLiftTime);

                if (Feedback.HasLiftTime)
                {
                    Feedback.LiftTime = EasyEditorField.Value(
                        EditorHelper.TempContent("生命时间"),
                        Feedback.LiftTime);
                }
            }));

            PropertiesGroups.Add(new PropertiesGroupInfo("变换设置", rect =>
            {
                Feedback.Parent = EasyEditorField.UnityObject(
                    EditorHelper.TempContent("父级"),
                    Feedback.Parent);
                    
                Feedback.Relative = EasyEditorField.UnityObject(
                    EditorHelper.TempContent("相对"),
                    Feedback.Relative);
                    
                Feedback.Position = EasyEditorField.Value(
                    EditorHelper.TempContent("坐标"),
                    Feedback.Position);
                    
                Feedback.Rotation = EasyEditorField.Value(
                    EditorHelper.TempContent("旋转"),
                    Feedback.Rotation);
                    
                Feedback.LocalScale = EasyEditorField.Value(
                    EditorHelper.TempContent("局部缩放"),
                    Feedback.LocalScale);
            }));
        }
    }
}
