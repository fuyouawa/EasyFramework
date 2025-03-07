using System;
using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EasyFramework.ToolKit.Editor
{
    public class ViewBinderEditorConfigDrawer : FoldoutValueDrawer<ViewBinderEditorConfig>
    {
        private ViewBinderSettings _settings;

        private InspectorProperty _propertyOfComment;

        protected override void Initialize()
        {
            base.Initialize();

            _settings = ViewBinderSettings.Instance;
            _propertyOfComment = Property.Children[nameof(ViewBinderEditorConfig.Comment)];
        }

        protected override GUIContent GetLabel(GUIContent label)
        {
            return EditorHelper.TempContent("绑定配置");
        }

        protected override void OnContentGUI(Rect headerRect)
        {
            foreach (var component in Property.Components)
            {
                EnsureInitialize(component.Property);
            }

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
                    ViewBinderUtility.GetSpecficableBindTypes(val.SpecificBindType),
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
            EditorGUILayout.TextField("实际变量名称", ((IViewBinder)comp).GetBindName());
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

            _propertyOfComment.Draw(EditorHelper.TempContent("注释"));

            if (GUILayout.Button("恢复默认值"))
            {
                UseDefault(Property);
            }
        }

        private void EnsureInitialize(InspectorProperty property)
        {
            var comp = GetTargetComponent(property);
            var cfg = property.WeakSmartValue<ViewBinderEditorConfig>();

            if (!cfg.IsInitialized)
            {
                UseDefault(property);
                cfg.IsInitialized = true;

                ValueChanged(comp);
            }
        }

        private void UseDefault(InspectorProperty property)
        {
            var comp = GetTargetComponent(property);
            var cfg = property.WeakSmartValue<ViewBinderEditorConfig>();

            cfg.BindName = comp.gameObject.name;
            var bindableComps = ((IViewBinder)comp).GetBindableComponentTypes();
            cfg.BindComponentType = bindableComps[0];

            cfg.SpecificBindType = ViewBinderUtility.GetDefaultSpecialType(
                ViewBinderUtility.GetSpecficableBindTypes(cfg.BindComponentType));

            cfg.BindGameObject = _settings.Default.BindGameObject;
            cfg.BindAccess = _settings.Default.BindAccess;
            cfg.AutoBindName = _settings.Default.AutoBindName;
            cfg.ProcessBindName = _settings.Default.ProcessBindName;
            cfg.UseDocumentComment = _settings.Default.UseDocumentComment;
            cfg.AutoAddParaToComment = _settings.Default.AutoAddParaToComment;
            cfg.Comment = _settings.Default.Comment;
        }

        private void ValueChanged(Object target)
        {
            EditorUtility.SetDirty(target);
        }

        private static Component GetTargetComponent(InspectorProperty property)
        {
            return property.Parent.Parent.WeakSmartValue<Component>();
        }
    }
}
