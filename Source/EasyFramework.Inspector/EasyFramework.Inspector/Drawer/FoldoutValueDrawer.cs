using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyGameFramework.Editor
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
                OnContentGUI = OnContentGUI
            };

            Property.State.Expanded = EasyEditorGUI.FoldoutGroup(config);
        }

        protected virtual string GetLabel(GUIContent label)
        {
            return label.text;
        }

        protected virtual string GetRightLabel(GUIContent label)
        {
            return string.Empty;
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
