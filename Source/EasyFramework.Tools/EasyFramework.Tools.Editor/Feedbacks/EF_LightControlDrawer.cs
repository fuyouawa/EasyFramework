using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_LightControlDrawer : AbstractEasyFeedbackDrawer<EF_LightControl>
    {
        private InspectorProperty _intensityProperty;
        private InspectorProperty _colorProperty;
        private InspectorProperty _rangeProperty;

        protected override void Initialize()
        {
            base.Initialize();
            _intensityProperty = Property.Children[nameof(EF_LightControl.Intensity)];
            _colorProperty = Property.Children[nameof(EF_LightControl.Color)];
            _rangeProperty = Property.Children[nameof(EF_LightControl.Range)];
        }

        protected override void DrawOtherPropertyLayout()
        {
            FreeExpand1 = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                FreeKey1, "光照控制", FreeExpand1)
            {
                OnContentGUI = rect =>
                {
                    Feedback.TargetLight = EasyEditorField.UnityObject(
                        EditorHelper.TempContent("目标光照"),
                        Feedback.TargetLight);
                    Feedback.Duration = EasyEditorField.Value(
                        EditorHelper.TempContent("持续时间"),
                        Feedback.Duration);
                    Feedback.DisableOnStop = EasyEditorField.Value(
                        EditorHelper.TempContent("停止时禁用"),
                        Feedback.DisableOnStop);
                    Feedback.RelativeValues = EasyEditorField.Value(
                        EditorHelper.TempContent("使用相对数值"),
                        Feedback.RelativeValues);
                }
            });

            FreeExpand2 = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                FreeKey2, "光照颜色", FreeExpand2)
            {
                OnContentGUI = rect =>
                {
                    Feedback.ModifyColor = EasyEditorField.Value(
                        EditorHelper.TempContent("修改颜色"),
                        Feedback.ModifyColor);

                    if (Feedback.ModifyColor)
                    {
                        _colorProperty.Draw(EditorHelper.TempContent("颜色"));
                    }
                }
            });

            FreeExpand3 = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                FreeKey3, "光照强度", FreeExpand3)
            {
                OnContentGUI = rect =>
                {
                    Feedback.ModifyIntensity = EasyEditorField.Value(
                        EditorHelper.TempContent("修改强度"),
                        Feedback.ModifyIntensity);

                    if (Feedback.ModifyColor)
                    {
                        _intensityProperty.Draw(EditorHelper.TempContent("强度"));
                    }
                }
            });

            FreeExpand4 = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                FreeKey4, "光照范围", FreeExpand4)
            {
                OnContentGUI = rect =>
                {
                    Feedback.ModifyRange = EasyEditorField.Value(
                        EditorHelper.TempContent("修改范围"),
                        Feedback.ModifyRange);

                    if (Feedback.ModifyRange)
                    {
                        _rangeProperty.Draw(EditorHelper.TempContent("范围"));
                    }
                }
            });
        }
    }
}
