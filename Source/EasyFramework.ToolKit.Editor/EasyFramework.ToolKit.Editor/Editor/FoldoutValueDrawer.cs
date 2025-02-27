using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public abstract class FoldoutValueDrawer<T> : OdinValueDrawer<T>
    {
        private FoldoutGroupConfig _foldoutGroupConfig;

        protected override void Initialize()
        {
            base.Initialize();

            _foldoutGroupConfig = new FoldoutGroupConfig(
                UniqueDrawerKey.Create(Property, this),
                GUIContent.none, Property.State.Expanded, OnContentGUI)
            {
                OnTitleBarGUI = OnTitleBarGUI,
                OnCoveredTitleBarGUI = OnCoveredTitleBarGUI,
            };
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            _foldoutGroupConfig.Label = GetLabel(label);
            _foldoutGroupConfig.RightLabelConfig = GetRightLabelConfig(label);

            _foldoutGroupConfig.Expand = Property.State.Expanded;
            Property.State.Expanded = EasyEditorGUI.FoldoutGroup(_foldoutGroupConfig);
        }

        protected virtual GUIContent GetLabel(GUIContent label)
        {
            return label;
        }

        private readonly LabelConfig _labelConfig = new LabelConfig();

        protected virtual LabelConfig GetRightLabelConfig(GUIContent label)
        {
            return _labelConfig;
        }

        protected virtual void OnCoveredTitleBarGUI(Rect headerRect)
        {
        }

        protected virtual void OnTitleBarGUI(Rect headerRect)
        {
        }

        protected virtual void OnContentGUI(Rect headerRect)
        {
            foreach (var child in Property.Children)
            {
                child.Draw();
            }
        }
    }
}
