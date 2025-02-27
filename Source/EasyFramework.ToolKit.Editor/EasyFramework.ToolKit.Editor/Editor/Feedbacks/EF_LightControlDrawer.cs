using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;

namespace EasyFramework.ToolKit.Editor
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

        protected override void PostBuildPropertiesGroups()
        {
            PropertiesGroups.Add(new PropertiesGroupInfo("光照控制", rect =>
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
            }));

            PropertiesGroups.Add(new PropertiesGroupInfo("光照颜色", rect =>
            {
                Feedback.ModifyColor = EasyEditorField.Value(
                    EditorHelper.TempContent("修改颜色"),
                    Feedback.ModifyColor);

                if (Feedback.ModifyColor)
                {
                    _colorProperty.Draw(EditorHelper.TempContent("颜色"));
                }
            }));

            PropertiesGroups.Add(new PropertiesGroupInfo("光照强度", rect =>
            {
                Feedback.ModifyIntensity = EasyEditorField.Value(
                    EditorHelper.TempContent("修改强度"),
                    Feedback.ModifyIntensity);

                if (Feedback.ModifyColor)
                {
                    _intensityProperty.Draw(EditorHelper.TempContent("强度"));
                }
            }));

            PropertiesGroups.Add(new PropertiesGroupInfo("光照范围", rect =>
            {
                Feedback.ModifyRange = EasyEditorField.Value(
                    EditorHelper.TempContent("修改范围"),
                    Feedback.ModifyRange);

                if (Feedback.ModifyRange)
                {
                    _rangeProperty.Draw(EditorHelper.TempContent("范围"));
                }
            }));
        }
    }
}
