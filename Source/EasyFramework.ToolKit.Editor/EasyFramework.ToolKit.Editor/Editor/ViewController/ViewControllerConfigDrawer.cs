using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class ViewControllerConfigDrawer : FoldoutValueDrawer<ViewControllerConfig>
    {
        protected override void Initialize()
        {
            var val = ValueEntry.SmartValue;
            if (!val.EditorConfig.IsInitialized)
            {
                val.EditorConfig.IsJustBound = true;
            }

            if (val.EditorConfig.IsJustBound)
            {
                Property.State.Expanded = true;
                val.EditorConfig.IsJustBound = false;
            }
            base.Initialize();
        }

        protected override GUIContent GetLabel(GUIContent label)
        {
            return EditorHelper.TempContent("视图控制器配置");
        }

        protected override void OnContentGUI(Rect headerRect)
        {
            var val = ValueEntry.SmartValue;
            if (!val.EditorConfig.IsInitialized)
            {
                var comp = GetTargetComponent(Property);
                ((IViewController)comp).UseDefault();
                val.EditorConfig.IsInitialized = true;
                EditorUtility.SetDirty(comp);
            }
            base.OnContentGUI(headerRect);
        }

        private static Component GetTargetComponent(InspectorProperty property)
        {
            return property.Parent.WeakSmartValue<Component>();
        }
    }
}
