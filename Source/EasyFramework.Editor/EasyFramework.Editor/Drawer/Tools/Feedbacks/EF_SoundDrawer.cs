using EasyFramework.Feedbacks;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor.Drawer
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_SoundDrawer : AbstractEasyFeedbackDrawer<EF_Sound>
    {
        private InspectorProperty _randomSfxProperty;

        protected override void Initialize()
        {
            base.Initialize();
            _randomSfxProperty = Property.Children[nameof(EF_Sound.RandomSfx)];
        }

        protected override void PostBuildPropertiesGroups()
        {
            PropertiesGroups.Add(new PropertiesGroupInfo("音效设置", rect =>
            {
                EasyEditorField.UnityObject(
                    EditorHelper.TempContent("音效"),
                    ref Feedback.Sfx);

                if (Feedback.Sfx == null)
                {
                    _randomSfxProperty.Draw(new GUIContent("随机音效"));
                }
            }));

            PropertiesGroups.Add(new PropertiesGroupInfo("播放方式", rect =>
            {
                EasyEditorField.Enum(
                    EditorHelper.TempContent("播放方式"),
                    ref Feedback.PlayMethod);

                if (Feedback.PlayMethod == EF_Sound.PlayMethods.Pool)
                {
                    EasyEditorField.Value(
                        EditorHelper.TempContent("池大小"),
                        ref Feedback.PoolSize);
                }
                
                EasyEditorField.Value(
                    EditorHelper.TempContent("反馈停止时也停止音效"),
                    ref Feedback.StopSoundOnFeedbackStop);
            }));

            PropertiesGroups.Add(new PropertiesGroupInfo("声音属性", rect =>
            {
                SirenixEditorFields.MinMaxSlider(
                    new GUIContent("响度范围"),
                    ref Feedback.MinVolume, ref Feedback.MaxVolume,
                    0f, 2f, true);

                SirenixEditorFields.MinMaxSlider(
                    new GUIContent("音高范围"),
                    ref Feedback.MinPitch, ref Feedback.MaxPitch,
                    -3f, 3f, true);

                EasyEditorField.UnityObject(
                    new GUIContent("混音器组"),
                    ref Feedback.SfxAudioMixerGroup);

                EasyEditorField.Value(
                    EditorHelper.TempContent("优先级"),
                    ref Feedback.Priority);
            }));

            PropertiesGroups.Add(new PropertiesGroupInfo("空间设置", rect =>
            {
                Feedback.PanStereo = EditorGUILayout.Slider(
                    new GUIContent("立体声平衡"),
                    Feedback.PanStereo, -1f, 1f);
                Feedback.SpatialBlend = EditorGUILayout.Slider(
                    new GUIContent("空间感"),
                    Feedback.SpatialBlend, 0f, 1f);
            }));

            PropertiesGroups.Add(new PropertiesGroupInfo("3D Sound Settings", rect =>
            {
                    Feedback.DopplerLevel = EditorGUILayout.Slider(
                        new GUIContent("多普勒效应"),
                        Feedback.DopplerLevel, 0f, 5f);
                    Feedback.Spread = EditorGUILayout.IntSlider(
                        new GUIContent("扩展度"),
                        Feedback.Spread, 0, 360);

                    Feedback.RolloffMode = EasyEditorField.Enum(
                        new GUIContent("衰减方式"),
                        Feedback.RolloffMode);

                    Feedback.MinDistance = EditorGUILayout.FloatField(
                        new GUIContent("最小距离"),
                        Feedback.MinDistance);
                    Feedback.MaxDistance = EditorGUILayout.FloatField(
                        new GUIContent("最大距离"),
                        Feedback.MaxDistance);

                    Feedback.UseCustomRolloffCurve = EditorGUILayout.Toggle(
                        new GUIContent("启用自定义衰减曲线"),
                        Feedback.UseCustomRolloffCurve);
                    if (Feedback.UseCustomRolloffCurve)
                    {
                        Feedback.CustomRolloffCurve = EditorGUILayout.CurveField(
                            new GUIContent("自定义衰减曲线"), Feedback.CustomRolloffCurve);
                    }

                    Feedback.UseSpatialBlendCurve = EditorGUILayout.Toggle(
                        new GUIContent("启用自定义空间混合曲线"),
                        Feedback.UseSpatialBlendCurve);
                    if (Feedback.UseSpatialBlendCurve)
                    {
                        Feedback.SpatialBlendCurve = EditorGUILayout.CurveField(
                            new GUIContent("自定义空间混合曲线"), Feedback.SpatialBlendCurve);
                    }

                    Feedback.UseReverbZoneMixCurve = EditorGUILayout.Toggle(
                        new GUIContent("启用自定义混响区混合曲线"),
                        Feedback.UseReverbZoneMixCurve);
                    if (Feedback.UseReverbZoneMixCurve)
                    {
                        Feedback.ReverbZoneMixCurve = EditorGUILayout.CurveField(
                            new GUIContent("自定义混响区混合曲线"), Feedback.ReverbZoneMixCurve);
                    }

                    Feedback.UseSpreadCurve = EditorGUILayout.Toggle(
                        new GUIContent("启用自定义扩展曲线"),
                        Feedback.UseSpreadCurve);
                    if (Feedback.UseSpreadCurve)
                    {
                        Feedback.SpreadCurve = EditorGUILayout.CurveField(
                            new GUIContent("自定义扩展曲线"), Feedback.SpreadCurve);
                    }
            }));
        }
    }
}
