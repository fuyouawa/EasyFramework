using System;
using System.Linq;
using EasyFramework.Core;
using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.StateUpdaters;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    [CustomEditor(typeof(ViewBinder))]
    [CanEditMultipleObjects]
    public class ViewBinderEditor : OdinEditor
    {
        private InspectorProperty _noOwningControllerProperty;
        private InspectorProperty _owningControllerProperty;
        private InspectorProperty _bindGameObjectProperty;
        private InspectorProperty _bindComponentTypeProperty;
        private InspectorProperty _specificBindTypeProperty;
        private InspectorProperty _bindAccessProperty;
        private InspectorProperty _bindNameProperty;
        private InspectorProperty _autoBindNameProperty;
        private InspectorProperty _processBindNameProperty;
        private InspectorProperty _useDocumentCommentProperty;
        private InspectorProperty _autoAddParaToCommentProperty;
        private InspectorProperty _commentProperty;
        private InspectorProperty _isInitializedProperty;

        private FoldoutGroupConfig _foldoutGroupConfig;

        protected override void OnEnable()
        {
            base.OnEnable();
            _noOwningControllerProperty = Tree.RootProperty.Children["_noOwningController"];
            _owningControllerProperty = Tree.RootProperty.Children["_owningController"];

            _bindGameObjectProperty = Tree.RootProperty.Children["_bindGameObject"];
            _bindComponentTypeProperty = Tree.RootProperty.Children["_bindComponentType"];
            _specificBindTypeProperty = Tree.RootProperty.Children["_specificBindType"];
            _bindAccessProperty = Tree.RootProperty.Children["_bindAccess"];
            _bindNameProperty = Tree.RootProperty.Children["_bindName"];
            _autoBindNameProperty = Tree.RootProperty.Children["_autoBindName"];
            _processBindNameProperty = Tree.RootProperty.Children["_processBindName"];
            _useDocumentCommentProperty = Tree.RootProperty.Children["_useDocumentComment"];
            _autoAddParaToCommentProperty = Tree.RootProperty.Children["_autoAddParaToComment"];
            _commentProperty = Tree.RootProperty.Children["_comment"];
            _isInitializedProperty = Tree.RootProperty.Children["_isInitialized"];

            _foldoutGroupConfig =
                new FoldoutGroupConfig(_bindGameObjectProperty, new GUIContent("绑定配置"), true, DrawSettings);
        }

        private void UnInitializeAll()
        {
            for (int i = 0; i < targets.Length; i++)
            {
                _isInitializedProperty.ValueEntry.WeakValues[i] = false;
            }
        }

        
        public static Type[] GetBindableComponentTypes(ViewBinder binder)
        {
            var types = binder.GetComponents<Component>()
                .Where(comp => comp != null)
                .Select(comp => comp.GetType())
                .Distinct()
                .ToArray();

            return types;
        }

        public static Type[] GetSpecficableBindTypes(Type bindType)
        {
            if (bindType == null)
            {
                return new Type[] { };
            }

            return bindType.GetAllBaseTypes(true, true)
                .Where(t => !t.IsInterface && t.IsSubclassOf(typeof(UnityEngine.Object)))
                .ToArray();
        }

        public static Type GetDefaultSpecialType(Type bindType)
        {
            var settings = ViewBinderSettings.Instance;
            var types = GetSpecficableBindTypes(bindType);
            foreach (var priority in settings.Priorities)
            {
                if (types.Contains(priority))
                {
                    return priority;
                }
            }
            return types.FirstOrDefault();
        }

        public static Type[] GetSortedBindableComponentTypes(ViewBinder binder)
        {
            var settings = ViewBinderSettings.Instance;
            var types = GetBindableComponentTypes(binder);
            Array.Sort(types, (a, b) =>
            {
                var indexA = settings.IndexByPriorityOf(a, false, true);
                var indexB = settings.IndexByPriorityOf(b, false, true);
            
                if (indexA < 0 && indexB >= 0)
                {
                    return 1;
                }
            
                if (indexB < 0 && indexA >= 0)
                {
                    return -1;
                }
            
                return indexA.CompareTo(indexB);
            });
            return types;
        }

        private void EnsureInitialize()
        {
            var settings = ViewBinderSettings.Instance;
            for (int i = 0; i < targets.Length; i++)
            {
                var binder = (ViewBinder)targets[i];
                if (!(bool)_isInitializedProperty.ValueEntry.WeakValues[i])
                {
                    var owners = binder.GetComponentsInParent(typeof(IViewController)).ToArray();
                    Component o = null;
                    if (owners.Length > 0)
                    {
                        o = owners[0];
                        // 如果游戏对象相同，说明是既持有Controller又持有Binder
                        // 绑定再上一级的controller(如果有)
                        if (o.gameObject == binder.gameObject)
                        {
                            if (owners.Length > 1)
                                o = owners[1];
                        }
                    }

                    _owningControllerProperty.ValueEntry.WeakValues[i] = o;

                    _bindNameProperty.ValueEntry.WeakValues[i] = binder.gameObject.name;

                    var bindType = GetSortedBindableComponentTypes(binder).FirstOrDefault();
                    _bindComponentTypeProperty.ValueEntry.WeakValues[i] = bindType;
                    
                    _specificBindTypeProperty.ValueEntry.WeakValues[i] = GetDefaultSpecialType(bindType);

                    _bindGameObjectProperty.ValueEntry.WeakValues[i] = settings.Default.BindGameObject;
                    _bindAccessProperty.ValueEntry.WeakValues[i] = settings.Default.BindAccess;
                    _autoBindNameProperty.ValueEntry.WeakValues[i] = settings.Default.AutoBindName;
                    _processBindNameProperty.ValueEntry.WeakValues[i] = settings.Default.ProcessBindName;
                    _useDocumentCommentProperty.ValueEntry.WeakValues[i] = settings.Default.UseDocumentComment;
                    _autoAddParaToCommentProperty.ValueEntry.WeakValues[i] = settings.Default.AutoAddParaToComment;
                    _commentProperty.ValueEntry.WeakValues[i] = settings.Default.Comment;

                    _isInitializedProperty.ValueEntry.WeakValues[i] = true;
                }
            }
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw(true);

            EnsureInitialize();

            _noOwningControllerProperty.DrawEx("不指定拥有者");

            if (_noOwningControllerProperty.ValueEntry.WeakSmartValueT<bool>())
            {
                GUIContent btnLabel;
                if (_owningControllerProperty.ValueEntry.IsAllSameWeakValues())
                {
                    var o = (Component)_owningControllerProperty.ValueEntry.WeakSmartValue;
                    btnLabel = o == null
                        ? EditorHelper.NoneSelectorBtnLabel
                        : EditorHelper.TempContent2(o.gameObject.name);
                }
                else
                {
                    btnLabel = EditorHelper.TempContent2("一");
                }

                var parentCtrls = targets.Cast<ViewBinder>()
                    .Select(binder => binder.GetComponentsInParent(typeof(IViewController), true))
                    .Aggregate((a, b) => a.Intersect(b).ToArray());

                EasyEditorGUI.DrawSelectorDropdown(
                    parentCtrls,
                    EditorHelper.TempContent("拥有者"),
                    btnLabel,
                    comp =>
                    {
                        _owningControllerProperty.ValueEntry.SetAllWeakValues(comp);
                    });
            }

            _foldoutGroupConfig.Expand = _bindGameObjectProperty.State.Expanded;
            _bindGameObjectProperty.State.Expanded = EasyEditorGUI.FoldoutGroup(_foldoutGroupConfig);

            Tree.EndDraw();
        }

        private void DrawSettings(Rect headerRect)
        {
            _bindGameObjectProperty.DrawEx("绑定游戏对象");

            if (!_bindGameObjectProperty.ValueEntry.WeakSmartValueT<bool>())
            {
            GUIContent btnLabel;
            if (_bindComponentTypeProperty.ValueEntry.IsAllSameWeakValues())
            {
                var type = (Type)_bindComponentTypeProperty.ValueEntry.WeakSmartValue;
                btnLabel = type == null
                    ? EditorHelper.NoneSelectorBtnLabel
                    : EditorHelper.TempContent2(type.GetNiceName());
            }
            else
            {
                btnLabel = EditorHelper.TempContent2("一");
            }

            var types = targets.Cast<ViewBinder>()
                .Select(GetSortedBindableComponentTypes)
                .Aggregate((a, b) => a.Intersect(b).ToArray());
            
            EasyEditorGUI.DrawSelectorDropdown(
                types,
                EditorHelper.TempContent("绑定组件"),
                btnLabel,
                t =>
                {
                    _bindComponentTypeProperty.ValueEntry.SetAllWeakValues(t);
                    _specificBindTypeProperty.ValueEntry.SetAllWeakValues(GetDefaultSpecialType(t));
                });


            if (_specificBindTypeProperty.ValueEntry.IsAllSameWeakValues())
            {
                var type = (Type)_specificBindTypeProperty.ValueEntry.WeakSmartValue;

                btnLabel = type == null
                    ? EditorHelper.NoneSelectorBtnLabel
                    : EditorHelper.TempContent2(type.GetNiceName());
            }
            else
            {
                btnLabel = EditorHelper.TempContent2("一");
            }

            types = targets.Cast<ViewBinder>()
                .Select((binder, i) => GetSpecficableBindTypes(((Type)_bindComponentTypeProperty.ValueEntry.WeakValues[i])))
                .Aggregate((a, b) => a.Intersect(b).ToArray());

            EasyEditorGUI.DrawSelectorDropdown(
                types,
                EditorHelper.TempContent("指定要绑定的类型"),
                btnLabel,
                t =>
                {
                    _specificBindTypeProperty.ValueEntry.SetAllWeakValues(t);
                });
            }
            
            _bindAccessProperty.DrawEx("访问权限");
            _bindNameProperty.DrawEx("绑定名称");
            _autoBindNameProperty.DrawEx("自动绑定名称", "绑定名称与游戏对象名称相同");

            _processBindNameProperty.DrawEx("处理绑定命名");

            EditorGUI.BeginDisabledGroup(true);

            string reallyName = null;

            foreach (var binder in targets.Cast<ViewBinder>())
            {
                if (reallyName == null)
                {
                    reallyName = binder.GetBindName();
                }
                else
                {
                    if (reallyName != binder.GetBindName())
                    {
                        reallyName = "一";
                        break;
                    }
                }
            }

            EditorGUILayout.TextField("实际变量名称", reallyName);

            EditorGUI.EndDisabledGroup();

            EasyEditorGUI.Title("注释设置");

            _useDocumentCommentProperty.DrawEx("使用文档注释");
            _autoAddParaToCommentProperty.DrawEx("自动添加注释段落");
            _commentProperty.DrawEx("注释");
        }
    }
}
