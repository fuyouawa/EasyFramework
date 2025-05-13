using EasyFramework.Core;
using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class ViewBinderEditorConfigDrawer : FoldoutValueDrawer<ViewBinderEditorConfig>
    {
        private InspectorProperty _commentProperty;

        protected override void Initialize()
        {
            var val = ValueEntry.SmartValue;
            if (!val.IsInitialized)
            {
                Property.State.Expanded = true;
            }

            base.Initialize();

            _commentProperty = Property.Children[nameof(ViewBinderEditorConfig.Comment)];
        }

        protected override GUIContent GetLabel(GUIContent label)
        {
            return EditorHelper.TempContent("绑定配置");
        }

        protected override void OnContentGUI(Rect headerRect)
        {
            var val = ValueEntry.SmartValue;
            var comp = GetTargetComponent(Property);
            var binder = (IViewBinder)comp;

            EditorGUI.BeginChangeCheck();

            val.BindGameObject = EditorGUILayout.Toggle(
                EditorHelper.TempContent("绑定游戏对象"),
                val.BindGameObject);

            if (!val.BindGameObject)
            {
                var btnLabel = val.BindComponentType == null
                    ? EditorHelper.NoneSelectorBtnLabel
                    : EditorHelper.TempContent2(val.BindComponentType.GetNiceName());

                EasyEditorGUI.DrawSelectorDropdown(
                    binder.GetBindableComponentTypes(),
                    EditorHelper.TempContent("绑定组件"),
                    btnLabel,
                    type =>
                    {
                        val.BindComponentType = type;
                        val.SpecificBindType = type != null
                            ? ViewBinderUtility.GetDefaultSpecialType(
                                ViewBinderUtility.GetSpecficableBindTypes(type))
                            : null;
                    },
                    type => type.GetNiceName());

                btnLabel = val.SpecificBindType == null
                    ? EditorHelper.NoneSelectorBtnLabel
                    : EditorHelper.TempContent2(val.SpecificBindType.GetNiceName());

                EasyEditorGUI.DrawSelectorDropdown(
                    ViewBinderUtility.GetSpecficableBindTypes(val.BindComponentType),
                    EditorHelper.TempContent("指定要绑定的类型"),
                    btnLabel,
                    type => val.SpecificBindType = type,
                    type => type.GetNiceName());
            }

            val.BindAccess = EnumSelector<ViewBindAccess>.DrawEnumField(
                EditorHelper.TempContent("访问权限"),
                val.BindAccess);
            val.BindName = EditorGUILayout.TextField(
                EditorHelper.TempContent("绑定名称"),
                val.BindName);

            val.AutoBindName = EditorGUILayout.Toggle(
                EditorHelper.TempContent("自动绑定名称", "绑定名称与游戏对象名称相同"),
                val.AutoBindName);

            val.ProcessBindName = EditorGUILayout.Toggle(
                EditorHelper.TempContent("处理绑定命名"),
                val.ProcessBindName);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("实际变量名称", binder.GetBindName());
            EditorGUI.EndDisabledGroup();

            EasyEditorGUI.Title("注释设置");

            val.UseDocumentComment = EditorGUILayout.Toggle(
                EditorHelper.TempContent("使用文档注释"),
                val.UseDocumentComment);

            if (val.UseDocumentComment)
            {
                val.AutoAddParaToComment = EditorGUILayout.Toggle(
                    EditorHelper.TempContent("自动添加注释段落"),
                    val.AutoAddParaToComment);
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(comp);
            }

            _commentProperty.Draw(EditorHelper.TempContent("注释"));
        }

        private static Component GetTargetComponent(InspectorProperty property)
        {
            return property.Parent.Parent.WeakSmartValue<Component>();
        }
    }
}
