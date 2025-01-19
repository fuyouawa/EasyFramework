using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_AudioSourceDrawer : AbstractEasyFeedbackDrawer<EF_AudioSource>
    {
        private InspectorProperty _targetAudioSource;
        private InspectorProperty _mode;
        private InspectorProperty _randomSfx;

        protected override void Initialize()
        {
            base.Initialize();
            _targetAudioSource = Property.Children["TargetAudioSource"];
            _mode = Property.Children["Mode"];
            _randomSfx = Property.Children["RandomSfx"];
        }

        protected override void DrawOtherPropertyLayout()
        {
            var value = ValueEntry.SmartValue;

            _targetAudioSource.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_targetAudioSource, this),
                "播放音频源", _targetAudioSource.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    if (value.TargetAudioSource == null)
                    {
                        EasyEditorGUI.MessageBox("音频源不能为空！", MessageType.Error);
                    }

                    _targetAudioSource.Draw(new GUIContent("目标音频源"));
                    _mode.Draw(new GUIContent("模式"));
                    _randomSfx.Draw(new GUIContent("随机音频"));

                    SirenixEditorFields.MinMaxSlider(
                        new GUIContent("响度范围"),
                        ref value.MinVolume, ref value.MaxVolume,
                        0f, 10f, true);

                    SirenixEditorFields.MinMaxSlider(
                        new GUIContent("音高范围"),
                        ref value.MinPitch, ref value.MaxPitch,
                        0f, 10f, true);
                }
            });
        }
    }
}
