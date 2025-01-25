using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyFramework.Editor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor.Drawer
{
    public class ViewModelInfoDrawer : OdinValueDrawer<ViewModelInfo>
    {
        private Component _component;
        private bool _isBuild;
        private ViewModelEditorInfo _editorInfo;
        private ViewModelInfo _info;

        private FoldoutGroupConfig _foldoutGroupConfig;
        private readonly ViewModelBuilder _builder = new ViewModelBuilder();
        private Type _classType;
        private ViewModelSettings _settings;

        protected override void Initialize()
        {
            base.Initialize();

            _component = Property.Parent.WeakSmartValue<Component>();
            _isBuild = _component as ViewModel == null;

            _info = ValueEntry.SmartValue;
            _editorInfo = _info.EditorData.Get<ViewModelEditorInfo>() ?? new ViewModelEditorInfo();
            if (_editorInfo == null)
            {
                _editorInfo = new ViewModelEditorInfo();
                _info.EditorData.Set(_editorInfo);
            }

            _foldoutGroupConfig = new FoldoutGroupConfig(
                UniqueDrawerKey.Create(Property, this),
                new GUIContent("视图模型"), true, OnContentGUI);

            _classType = _editorInfo.GetClassType();

            _settings = ViewModelSettings.Instance;
            if (!_editorInfo.IsInitialized)
            {
                _editorInfo.ClassName = _component.gameObject.name;
                _editorInfo.GenerateDir = _settings.GenerateDir;
                _editorInfo.Namespace = _settings.Namespace;
                _editorInfo.BaseClass.Value = typeof(MonoBehaviour);

                _editorInfo.IsInitialized = true;
                ValueChanged();
            }
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            _foldoutGroupConfig.Expand = Property.State.Expanded;
            Property.State.Expanded = EasyEditorGUI.FoldoutGroup(_foldoutGroupConfig);
        }

        private void OnContentGUI(Rect headerRect)
        {
            if (_editorInfo.ClassNameSameAsGameObjectName)
            {
                if (_editorInfo.ClassName != _component.gameObject.name)
                {
                    _editorInfo.ClassName = _component.gameObject.name;
                    ValueChanged();
                }
            }

            EditorGUILayout.LabelField("状态", _isBuild ? "已构建" : "未构建");

            EditorGUI.BeginDisabledGroup(_isBuild);
            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginChangeCheck();
            _editorInfo.GenerateDir = SirenixEditorFields.FolderPathField(
                new GUIContent("代码生成目录"),
                _editorInfo.GenerateDir, "Assets", false, false);
            if (EditorGUI.EndChangeCheck())
            {
                GUIHelper.ExitGUI(false);
            }

            _editorInfo.Namespace =
                EditorGUILayout.TextField("命名空间", _editorInfo.Namespace);

            _editorInfo.ClassNameSameAsGameObjectName =
                EditorGUILayout.Toggle("类名与游戏对象名称相同",
                    _editorInfo.ClassNameSameAsGameObjectName);

            ViewModelHelper.CheckIdentifier("类名", _editorInfo.ClassName);
            using (new EditorGUI.DisabledScope(_editorInfo.ClassNameSameAsGameObjectName))
            {
                _editorInfo.ClassName =
                    EditorGUILayout.TextField("类名", _editorInfo.ClassName);
            }

            var lbl = _editorInfo.BaseClass.Value == null
                ? "<None>"
                : _editorInfo.BaseClass.Value.FullName;

            EasyEditorGUI.DrawSelectorDropdown(
                new SelectorDropdownConfig<Type>(
                    EditorHelper.TempContent("父级"),
                    EditorHelper.TempContent2(lbl),
                    ViewModelHelper.BaseTypes,
                    t => _editorInfo.BaseClass.Value = t)
                {
                    MenuItemNameGetter = t => t.FullName
                });

            if (EditorGUI.EndChangeCheck())
            {
                ValueChanged();
            }

            EditorGUI.EndDisabledGroup();

            if (SirenixEditorGUI.SDFIconButton("构建", EditorGUIUtility.singleLineHeight,
                    SdfIconType.PencilFill))
            {
                if (!CheckBind())
                {
                    return;
                }

                _builder.Setup(_editorInfo, _component);
                _builder.Build();
            }

            if (_classType != null)
            {
                if (SirenixEditorGUI.SDFIconButton("绑定", EditorGUIUtility.singleLineHeight,
                        SdfIconType.Bezier))
                {
                    Bind();
                }
            }
        }

        private void ValueChanged()
        {
            _info.EditorData.Set(_editorInfo);
            EditorUtility.SetDirty(_component);
        }

        private void Bind()
        {
            Debug.Assert(_classType != null);

            if (!_component.GetComponent(_classType))
            {
                _component = _component.gameObject.AddComponent(_classType);
                ((IViewModel)_component).Info = _info;
            }

            var c = _component.GetComponent<ViewModel>();
            if (c != null)
                UnityEngine.Object.DestroyImmediate(c);

            var children = ViewModelHelper.GetChildren(_component.transform);

            var fields = _component.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<ByViewBinderAttribute>() != null)
                .ToArray();

            foreach (var child in children)
            {
                var comp = (Component)child;
                var binderEditorInfo = child.Info.EditorData.Get<ViewBinderEditorInfo>();
                var f = fields.FirstOrDefault(f => GetOriginName(f) == binderEditorInfo.Name);
                if (f == null)
                {
                    EditorUtility.DisplayDialog("错误",
                        $"绑定GameObject（{comp.gameObject.name}）失败，" +
                        $"视图模型中没有“{binderEditorInfo.Name}”，" +
                        $"可能需要重新生成视图模型！", "确认");
                    return;
                }

                var value = comp.GetComponent(f.FieldType);
                if (value == null)
                {
                    EditorUtility.DisplayDialog("错误",
                        $"绑定GameObject（{comp.gameObject.name}）失败，" +
                        $"没有组件“{f.FieldType}”", "确认");
                    return;
                }

                f.SetValue(_component, value);
            }

            return;

            string GetOriginName(FieldInfo field) =>
                field.GetCustomAttribute<ByViewBinderAttribute>().OriginName;
        }

        private bool CheckBind()
        {
            string error = ViewModelHelper.GetIdentifierError("类名", _editorInfo.ClassName);
            if (error.IsNotNullOrEmpty())
            {
                EditorUtility.DisplayDialog("错误", $"类名不规范：{error}", "确认");
                return false;
            }

            var children = ViewModelHelper.GetChildren(_component.transform);

            var nameCheck = new HashSet<string>();
            foreach (var child in children)
            {
                var comp = (Component)child;
                var binderEditorInfo = child.Info.EditorData.Get<ViewBinderEditorInfo>();
                error = ViewModelHelper.GetIdentifierError("变量名称", binderEditorInfo.Name);
                if (error.IsNotNullOrEmpty())
                {
                    EditorUtility.DisplayDialog("错误", $"绑定“{comp.gameObject.name}”出现错误：{error}", "确认");
                    return false;
                }

                if (!nameCheck.Add(binderEditorInfo.Name))
                {
                    EditorUtility.DisplayDialog("错误",
                        $"绑定“{comp.gameObject.name}”出现错误：重复的变量名称（{binderEditorInfo.Name}）", "确认");
                    return false;
                }
            }

            return true;
        }
    }
}
