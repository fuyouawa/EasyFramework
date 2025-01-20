using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.Inspector
{
    public abstract class FoldoutValueDrawer<T> : OdinValueDrawer<T>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var config = new FoldoutGroupConfig(UniqueDrawerKey.Create(Property, this), GetLabel(label),
                Property.State.Expanded)
            {
                OnTitleBarGUI = OnTitleBarGUI,
                OnCoveredTitleBarGUI = OnCoveredTitleBarGUI,
                OnContentGUI = OnContentGUI,
                RightLabelConfig = GetRightLabelConfig(label),
            };

            Property.State.Expanded = EasyEditorGUI.FoldoutGroup(config);
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
