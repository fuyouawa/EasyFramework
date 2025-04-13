using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;

namespace EasyFramework.ToolKit.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class GameObjectSetActiveFeedbackDrawer : AbstractFeedbackDrawer<GameObjectSetActiveFeedback>
    {
        protected override void PostBuildPropertiesGroups()
        {
            PropertiesGroups.Add(new PropertiesGroupInfo("激活设置", rect =>
            {
                Feedback.Target = EasyEditorField.UnityObject(
                    EditorHelper.TempContent("目标对象"),
                    Feedback.Target);

                Feedback.ActiveToSet = EasyEditorField.Value(
                    EditorHelper.TempContent("激活设置"),
                    Feedback.ActiveToSet);
            }));
        }
    }
}
