using EasyFramework.Editor;
using EasyFramework.ToolKit;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;

namespace EasyFramework.ToolKit.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class AudioSourceFeedbackDrawer : AbstractFeedbackDrawer<AudioSourceFeedback>
    {
        private InspectorProperty _randomSfxProperty;

        protected override void Initialize()
        {
            base.Initialize();
            _randomSfxProperty = Property.Children["RandomSfx"];
        }

        protected override void PostBuildPropertiesGroups()
        {
            PropertiesGroups.Add(new PropertiesGroupInfo("音频源设置", rect =>
            {
                if (Feedback.TargetAudioSource == null && Feedback.RandomSfx.IsNullOrEmpty())
                {
                    EasyEditorGUI.MessageBox("音频源不能为空！", MessageType.Error);
                }

                Feedback.TargetAudioSource = EasyEditorField.UnityObject(
                    EditorHelper.TempContent("音频源"),
                    Feedback.TargetAudioSource);

                if (Feedback.TargetAudioSource == null)
                {
                    _randomSfxProperty.Draw(EditorHelper.TempContent("随机音频"));
                }

                Feedback.Mode = EasyEditorField.Enum(
                    EditorHelper.TempContent("模式"),
                    Feedback.Mode);
            }));

            PropertiesGroups.Add(new PropertiesGroupInfo("声音属性", rect =>
            {
                SirenixEditorFields.MinMaxSlider(
                    EditorHelper.TempContent("响度范围"),
                    ref Feedback.MinVolume, ref Feedback.MaxVolume,
                    0f, 2f, true);

                SirenixEditorFields.MinMaxSlider(
                    EditorHelper.TempContent("音高范围"),
                    ref Feedback.MinPitch, ref Feedback.MaxPitch,
                    -3f, 3f, true);
            }));
        }
    }
}
