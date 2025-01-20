using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_GameObjectSetActiveDrawer : AbstractEasyFeedbackDrawer<EF_GameObjectSetActive>
    {
        protected override void DrawOtherPropertyLayout()
        {
            FreeExpand1 = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                FreeKey1, "激活设置", FreeExpand1)
            {
                OnContentGUI = rect =>
                {
                    Feedback.Target = EasyEditorField.UnityObject(
                        EditorHelper.TempContent("目标对象"),
                        Feedback.Target);

                    Feedback.ActiveToSet = EasyEditorField.Value(
                        EditorHelper.TempContent("激活设置"),
                        Feedback.ActiveToSet);
                }
            });
        }
    }
}
