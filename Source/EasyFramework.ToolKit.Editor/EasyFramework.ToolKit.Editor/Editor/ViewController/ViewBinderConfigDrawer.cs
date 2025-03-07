using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class ViewBinderConfigDrawer : OdinValueDrawer<ViewBinderConfig>
    {
        private InspectorProperty _propertyOfEditorConfig;

        protected override void Initialize()
        {
            _propertyOfEditorConfig = Property.Children[nameof(ViewBinderConfig.EditorConfig)];
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var val = ValueEntry.SmartValue;

            var comp = GetTargetComponent(Property);

            bool hasChange = false;
            if (!val.EditorConfig.IsInitialized)
            {
                ((IViewBinder)comp).UseDefault();
                val.EditorConfig.IsInitialized = true;
                hasChange = true;
            }

            var owners = comp.gameObject.GetComponentsInParent(typeof(IViewController));

            var btnLabel = val.OwnerController == null
                ? EditorHelper.NoneSelectorBtnLabel
                : EditorHelper.TempContent2(val.OwnerController.gameObject.name);

            EditorGUI.BeginChangeCheck();
            EasyEditorGUI.DrawSelectorDropdown(
                owners,
                EditorHelper.TempContent("拥有者"),
                btnLabel,
                value => val.OwnerController = value,
                value => value.gameObject.name);
            if (EditorGUI.EndChangeCheck())
            {
                hasChange = true;
            }

            _propertyOfEditorConfig.Draw(GUIContent.none);

            if (GUILayout.Button("恢复默认值"))
            {
                ((IViewBinder)comp).UseDefault();
                hasChange = true;
            }

            if (hasChange)
            {
                EditorUtility.SetDirty(comp);
            }
        }

        private static Component GetTargetComponent(InspectorProperty property)
        {
            return property.Parent.WeakSmartValue<Component>();
        }
    }
}
