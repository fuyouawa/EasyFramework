using System;
using System.Linq;
using EasyFramework.Core;
using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    [CustomEditor(typeof(Binder))]
    [CanEditMultipleObjects]
    public class BinderEditor : OdinEditor
    {
        private InspectorProperty _owningBuildersProperty;
        private InspectorProperty _bindGameObjectProperty;
        private InspectorProperty _bindComponentTypeProperty;
        private InspectorProperty _specificBindTypeProperty;
        private InspectorProperty _bindAccessProperty;
        private InspectorProperty _bindNameProperty;
        private InspectorProperty _useGameObjectNameProperty;
        private InspectorProperty _processBindNameProperty;
        private InspectorProperty _useDocumentCommentProperty;
        private InspectorProperty _autoAddParaToCommentProperty;
        private InspectorProperty _commentProperty;
        private InspectorProperty _isInitializedProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _owningBuildersProperty = Tree.RootProperty.Children["_owningBuilders"];
            _bindGameObjectProperty = Tree.RootProperty.Children["_bindGameObject"];
            _bindComponentTypeProperty = Tree.RootProperty.Children["_bindComponentType"];
            _specificBindTypeProperty = Tree.RootProperty.Children["_specificBindType"];
            _bindAccessProperty = Tree.RootProperty.Children["_bindAccess"];
            _bindNameProperty = Tree.RootProperty.Children["_bindName"];
            _useGameObjectNameProperty = Tree.RootProperty.Children["_useGameObjectName"];
            _processBindNameProperty = Tree.RootProperty.Children["_processBindName"];
            _useDocumentCommentProperty = Tree.RootProperty.Children["_useDocumentComment"];
            _autoAddParaToCommentProperty = Tree.RootProperty.Children["_autoAddParaToComment"];
            _commentProperty = Tree.RootProperty.Children["_comment"];
            _isInitializedProperty = Tree.RootProperty.Children["_isInitialized"];
        }

        private void UnInitializeAll()
        {
            for (int i = 0; i < targets.Length; i++)
            {
                _isInitializedProperty.ValueEntry.WeakValues[i] = false;
            }
        }


        public static Type[] GetBindableComponentTypes(Binder binder)
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
            var settings = BinderSettings.Instance;
            var types = GetSpecficableBindTypes(bindType);
            foreach (var priority in settings.PriorityTypes)
            {
                if (types.Contains(priority))
                {
                    return priority;
                }
            }

            return types.FirstOrDefault();
        }

        public static Type[] GetSortedBindableComponentTypes(Binder binder)
        {
            var settings = BinderSettings.Instance;
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
            var settings = BinderSettings.Instance;
            for (int i = 0; i < targets.Length; i++)
            {
                var binder = (Binder)targets[i];
                if (!(bool)_isInitializedProperty.ValueEntry.WeakValues[i])
                {
                    var b = binder.GetComponentInParent<Builder>();
                    _owningBuildersProperty.ValueEntry.WeakValues[i] = b == null
                        ? new Builder[] { }
                        : new Builder[] { b };

                    _bindNameProperty.ValueEntry.WeakValues[i] = binder.gameObject.name;

                    var bindType = GetSortedBindableComponentTypes(binder).FirstOrDefault();
                    _bindComponentTypeProperty.ValueEntry.WeakValues[i] = bindType;

                    _specificBindTypeProperty.ValueEntry.WeakValues[i] = GetDefaultSpecialType(bindType);

                    _bindGameObjectProperty.ValueEntry.WeakValues[i] = settings.DefaultBindGameObject;
                    _bindAccessProperty.ValueEntry.WeakValues[i] = settings.DefaultBindAccess;
                    _useGameObjectNameProperty.ValueEntry.WeakValues[i] = settings.DefaultUseGameObjectName;
                    _processBindNameProperty.ValueEntry.WeakValues[i] = settings.DefaultProcessBindName;
                    _useDocumentCommentProperty.ValueEntry.WeakValues[i] = settings.DefaultUseDocumentComment;
                    _autoAddParaToCommentProperty.ValueEntry.WeakValues[i] = settings.DefaultAutoAddParaToComment;
                    _commentProperty.ValueEntry.WeakValues[i] = settings.DefaultComment;

                    _isInitializedProperty.ValueEntry.WeakValues[i] = true;
                }
            }
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw(true);

            EnsureInitialize();

            _owningBuildersProperty.DrawEx("绑定列表");

            _bindGameObjectProperty.State.Expanded = EasyEditorGUI.FoldoutGroup(
                _bindGameObjectProperty,
                "绑定设置",
                _bindGameObjectProperty.State.Expanded,
                DrawSettings);

            Tree.EndDraw();
        }

        private void DrawSettings(Rect headerRect)
        {
            _bindGameObjectProperty.DrawEx("绑定游戏对象");

            if (!_bindGameObjectProperty.ValueEntry.WeakSmartValueT<bool>())
            {
                EasyEditorGUI.DrawSelectorDropdown(
                    () => targets.Cast<Binder>()
                        .Select(GetSortedBindableComponentTypes)
                        .Aggregate((a, b) => a.Intersect(b).ToArray()),
                    EditorHelper.TempContent("绑定组件"),
                    _bindComponentTypeProperty.GetSmartLabel(),
                    t =>
                    {
                        _bindComponentTypeProperty.ValueEntry.SetAllWeakValues(t);
                        _specificBindTypeProperty.ValueEntry.SetAllWeakValues(GetDefaultSpecialType(t));
                    });


                EasyEditorGUI.DrawSelectorDropdown(
                    () => targets.Cast<Binder>()
                        .Select((binder, i) =>
                            GetSpecficableBindTypes(((Type)_bindComponentTypeProperty.ValueEntry.WeakValues[i])))
                        .Aggregate((a, b) => a.Intersect(b).ToArray()),
                    EditorHelper.TempContent("指定要绑定的类型"),
                    _specificBindTypeProperty.GetSmartLabel(),
                    t => { _specificBindTypeProperty.ValueEntry.SetAllWeakValues(t); });
            }

            _bindAccessProperty.DrawEx("访问权限");
            _bindNameProperty.DrawEx("绑定名称");
            _useGameObjectNameProperty.DrawEx("使用游戏对象名称", "绑定名称与游戏对象名称相同");

            _processBindNameProperty.DrawEx("处理绑定命名");

            EditorGUI.BeginDisabledGroup(true);

            string reallyName = null;

            if (targets.Cast<Binder>().AllSame())
            {
                reallyName = ((Binder)target).GetBindName();
            }
            else
            {
                reallyName = "一";
            }

            EditorGUILayout.TextField("实际变量名称", reallyName);

            EditorGUI.EndDisabledGroup();

            EasyEditorGUI.Title("注释设置");

            _useDocumentCommentProperty.DrawEx("使用文档注释");
            _autoAddParaToCommentProperty.DrawEx("自动添加注释段落");
            _commentProperty.DrawEx("注释");

            if (GUILayout.Button("恢复默认值"))
            {
                UnInitializeAll();
            }
        }
    }
}
