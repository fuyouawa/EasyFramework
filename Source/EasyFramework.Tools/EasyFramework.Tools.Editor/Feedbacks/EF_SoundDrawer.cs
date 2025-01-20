using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_SoundDrawer : AbstractEasyFeedbackDrawer<EF_Sound>
    {
        private InspectorProperty _sfx;
        private InspectorProperty _randomSfx;
        private InspectorProperty _playMethod;
        private InspectorProperty _poolSize;
        private InspectorProperty _stopSoundOnFeedbackStop;

        private InspectorProperty _minVolume;
        private InspectorProperty _priority;

        private InspectorProperty _panStereo;

        private InspectorProperty _dopplerLevel;

        protected override void Initialize()
        {
            base.Initialize();
            _sfx = Property.Children["Sfx"];
            _randomSfx = Property.Children["RandomSfx"];
            _playMethod = Property.Children["PlayMethod"];
            _poolSize = Property.Children["PoolSize"];
            _stopSoundOnFeedbackStop = Property.Children["StopSoundOnFeedbackStop"];
            _minVolume = Property.Children["MinVolume"];
            _priority = Property.Children["Priority"];

            _panStereo = Property.Children["PanStereo"];

            _dopplerLevel = Property.Children["DopplerLevel"];
        }

        protected override void DrawOtherPropertyLayout()
        {
            var value = ValueEntry.SmartValue;

            _sfx.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_sfx, this),
                "音效设置", _sfx.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    _sfx.Draw(new GUIContent("音效"));
                    if (value.Sfx == null)
                    {
                        _randomSfx.Draw(new GUIContent("随机音效"));
                    }
                }
            });

            _playMethod.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_playMethod, this),
                "播放方式", _playMethod.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    _playMethod.Draw(new GUIContent("播放方式"));
                    if (value.PlayMethod == EF_Sound.PlayMethods.Pool)
                    {
                        _poolSize.Draw(new GUIContent("池大小"));
                    }

                    _stopSoundOnFeedbackStop.Draw(new GUIContent("反馈停止时也停止音效"));
                }
            });

            _minVolume.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_minVolume, this),
                "声音属性", _minVolume.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    SirenixEditorFields.MinMaxSlider(
                        new GUIContent("响度范围"),
                        ref value.MinVolume, ref value.MaxVolume,
                        0f, 2f, true);

                    SirenixEditorFields.MinMaxSlider(
                        new GUIContent("音高范围"),
                        ref value.MinPitch, ref value.MaxPitch,
                        -3f, 3f, true);

                    value.SfxAudioMixerGroup = EasyEditorField.UnityObject(
                        new GUIContent("混音器组"), value.SfxAudioMixerGroup, true);

                    _priority.Draw(new GUIContent("优先级"));
                }
            });

            _panStereo.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_panStereo, this),
                "空间设置", _panStereo.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    value.PanStereo = EditorGUILayout.Slider(
                        new GUIContent("立体声平衡"),
                        value.PanStereo, -1f, 1f);
                    value.SpatialBlend = EditorGUILayout.Slider(
                        new GUIContent("空间感"),
                        value.SpatialBlend, 0f, 1f);
                }
            });

            _dopplerLevel.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_dopplerLevel, this),
                "3D Sound Settings", _dopplerLevel.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    value.DopplerLevel = EditorGUILayout.Slider(
                        new GUIContent("多普勒效应"),
                        value.DopplerLevel, 0f, 5f);
                    value.Spread = EditorGUILayout.IntSlider(
                        new GUIContent("扩展度"),
                        value.Spread, 0, 360);

                    value.RolloffMode = EasyEditorField.Enum(
                        new GUIContent("衰减方式"),
                        value.RolloffMode);

                    value.MinDistance = EditorGUILayout.FloatField(
                        new GUIContent("最小距离"),
                        value.MinDistance);
                    value.MaxDistance = EditorGUILayout.FloatField(
                        new GUIContent("最大距离"),
                        value.MaxDistance);

                    value.UseCustomRolloffCurve = EditorGUILayout.Toggle(
                        new GUIContent("启用自定义衰减曲线"),
                        value.UseCustomRolloffCurve);
                    if (value.UseCustomRolloffCurve)
                    {
                        value.CustomRolloffCurve = EditorGUILayout.CurveField(
                            new GUIContent("自定义衰减曲线"), value.CustomRolloffCurve);
                    }

                    value.UseSpatialBlendCurve = EditorGUILayout.Toggle(
                        new GUIContent("启用自定义空间混合曲线"),
                        value.UseSpatialBlendCurve);
                    if (value.UseSpatialBlendCurve)
                    {
                        value.SpatialBlendCurve = EditorGUILayout.CurveField(
                            new GUIContent("自定义空间混合曲线"), value.SpatialBlendCurve);
                    }

                    value.UseReverbZoneMixCurve = EditorGUILayout.Toggle(
                        new GUIContent("启用自定义混响区混合曲线"),
                        value.UseReverbZoneMixCurve);
                    if (value.UseReverbZoneMixCurve)
                    {
                        value.ReverbZoneMixCurve = EditorGUILayout.CurveField(
                            new GUIContent("自定义混响区混合曲线"), value.ReverbZoneMixCurve);
                    }

                    value.UseSpreadCurve = EditorGUILayout.Toggle(
                        new GUIContent("启用自定义扩展曲线"),
                        value.UseSpreadCurve);
                    if (value.UseSpreadCurve)
                    {
                        value.SpreadCurve = EditorGUILayout.CurveField(
                            new GUIContent("自定义扩展曲线"), value.SpreadCurve);
                    }
                }
            });
        }
    }
}
