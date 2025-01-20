using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_LightControlDrawer : AbstractEasyFeedbackDrawer<EF_LightControl>
    {
        private InspectorProperty _targetLight;
        private InspectorProperty _duration;
        private InspectorProperty _disableOnStop;
        private InspectorProperty _relativeValues;

        private InspectorProperty _modifyColor;
        private InspectorProperty _color;
        private InspectorProperty _modifyIntensity;
        private InspectorProperty _intensity;
        private InspectorProperty _modifyRange;
        private InspectorProperty _range;
    
        protected override void Initialize()
        {
            base.Initialize();
            _targetLight = Property.Children["TargetLight"];
            _duration = Property.Children["Duration"];
            _disableOnStop = Property.Children["DisableOnStop"];
            _relativeValues = Property.Children["RelativeValues"];

            _modifyColor = Property.Children["ModifyColor"];
            _color = Property.Children["Color"];
            _modifyIntensity = Property.Children["ModifyIntensity"];
            _intensity = Property.Children["Intensity"];
            _modifyRange = Property.Children["ModifyRange"];
            _range = Property.Children["Range"];
        }
    
        protected override void DrawOtherPropertyLayout()
        {
            var val = ValueEntry.SmartValue;
            
            _targetLight.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_targetLight, this),
                "光照控制", _targetLight.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    _targetLight.Draw(new GUIContent("目标光照"));
                    _duration.Draw(new GUIContent("持续时间"));
                    _disableOnStop.Draw(new GUIContent("停止时禁用"));
                    _relativeValues.Draw(new GUIContent("使用相对数值"));
                }
            });

            _modifyColor.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_modifyColor, this),
                "光照颜色", _modifyColor.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    _modifyColor.Draw(new GUIContent("修改颜色"));

                    if (val.ModifyColor)
                    {
                        _color.Draw(new GUIContent("颜色"));
                    }
                }
            });

            _modifyIntensity.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_modifyIntensity, this),
                "光照强度", _modifyIntensity.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    _modifyIntensity.Draw(new GUIContent("修改强度"));

                    if (val.ModifyIntensity)
                    {
                        _intensity.Draw(new GUIContent("强度"));
                    }
                }
            });

            _modifyRange.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_modifyRange, this),
                "光照范围", _modifyRange.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    _modifyRange.Draw(new GUIContent("修改范围"));

                    if (val.ModifyRange)
                    {
                        _range.Draw(new GUIContent("范围"));
                    }
                }
            });
        }
    }
}
