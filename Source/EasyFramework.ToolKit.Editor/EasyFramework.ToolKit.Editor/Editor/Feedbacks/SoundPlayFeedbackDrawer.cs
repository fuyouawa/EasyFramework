using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class SoundPlayFeedbackDrawer : AbstractFeedbackDrawer<SoundPlayFeedback>
    {
        private InspectorProperty _propertyOfRandomSfx;
        private InspectorProperty _propertyOfVolume;
        private InspectorProperty _propertyOfPitch;
        private InspectorProperty _propertyOfDistance;

        protected override void Initialize()
        {
            base.Initialize();
            _propertyOfRandomSfx = Property.Children[nameof(SoundPlayFeedback.RandomSfx)];
            _propertyOfVolume = Property.Children[nameof(SoundPlayFeedback.Volume)];
            _propertyOfPitch = Property.Children[nameof(SoundPlayFeedback.Pitch)];
            _propertyOfDistance = Property.Children[nameof(SoundPlayFeedback.Distance)];
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
                    _propertyOfRandomSfx.Draw(EditorHelper.TempContent("随机音效"));
                }
            }));

            PropertiesGroups.Add(new PropertiesGroupInfo("播放方式", rect =>
            {
                EasyEditorField.Enum(
                    EditorHelper.TempContent("播放方式"),
                    ref Feedback.PlayMethod);

                if (Feedback.PlayMethod == SoundPlayFeedback.PlayMethods.Pool)
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
                _propertyOfVolume.Draw(EditorHelper.TempContent("响度"));
                _propertyOfPitch.Draw(EditorHelper.TempContent("音高"));

                EasyEditorField.UnityObject(
                    EditorHelper.TempContent("混音器组"),
                    ref Feedback.SfxAudioMixerGroup);

                EasyEditorField.Value(
                    EditorHelper.TempContent("优先级"),
                    ref Feedback.Priority);
            }));

            PropertiesGroups.Add(new PropertiesGroupInfo("空间设置", rect =>
            {
                Feedback.PanStereo = EditorGUILayout.Slider(
                    EditorHelper.TempContent("立体声平衡"),
                    Feedback.PanStereo, -1f, 1f);
                Feedback.SpatialBlend = EditorGUILayout.Slider(
                    EditorHelper.TempContent("空间感"),
                    Feedback.SpatialBlend, 0f, 1f);
            }));

            PropertiesGroups.Add(new PropertiesGroupInfo("3D Sound Settings", rect =>
            {
                    Feedback.DopplerLevel = EditorGUILayout.Slider(
                        EditorHelper.TempContent("多普勒效应"),
                        Feedback.DopplerLevel, 0f, 5f);
                    Feedback.Spread = EditorGUILayout.IntSlider(
                        EditorHelper.TempContent("扩展度"),
                        Feedback.Spread, 0, 360);

                    Feedback.RolloffMode = EasyEditorField.Enum(
                        EditorHelper.TempContent("衰减方式"),
                        Feedback.RolloffMode);

                    _propertyOfDistance.Draw(EditorHelper.TempContent("距离"));

                    Feedback.UseCustomRolloffCurve = EditorGUILayout.Toggle(
                        EditorHelper.TempContent("启用自定义衰减曲线"),
                        Feedback.UseCustomRolloffCurve);
                    if (Feedback.UseCustomRolloffCurve)
                    {
                        Feedback.CustomRolloffCurve = EditorGUILayout.CurveField(
                            EditorHelper.TempContent("自定义衰减曲线"), Feedback.CustomRolloffCurve);
                    }

                    Feedback.UseSpatialBlendCurve = EditorGUILayout.Toggle(
                        EditorHelper.TempContent("启用自定义空间混合曲线"),
                        Feedback.UseSpatialBlendCurve);
                    if (Feedback.UseSpatialBlendCurve)
                    {
                        Feedback.SpatialBlendCurve = EditorGUILayout.CurveField(
                            EditorHelper.TempContent("自定义空间混合曲线"), Feedback.SpatialBlendCurve);
                    }

                    Feedback.UseReverbZoneMixCurve = EditorGUILayout.Toggle(
                        EditorHelper.TempContent("启用自定义混响区混合曲线"),
                        Feedback.UseReverbZoneMixCurve);
                    if (Feedback.UseReverbZoneMixCurve)
                    {
                        Feedback.ReverbZoneMixCurve = EditorGUILayout.CurveField(
                            EditorHelper.TempContent("自定义混响区混合曲线"), Feedback.ReverbZoneMixCurve);
                    }

                    Feedback.UseSpreadCurve = EditorGUILayout.Toggle(
                        EditorHelper.TempContent("启用自定义扩展曲线"),
                        Feedback.UseSpreadCurve);
                    if (Feedback.UseSpreadCurve)
                    {
                        Feedback.SpreadCurve = EditorGUILayout.CurveField(
                            EditorHelper.TempContent("自定义扩展曲线"), Feedback.SpreadCurve);
                    }
            }));
        }
    }
}
