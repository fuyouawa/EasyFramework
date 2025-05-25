using System;
using System.Linq;
using EasyFramework.Core;
using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    [CustomEditor(typeof(Builder))]
    [CanEditMultipleObjects]
    public class BuilderEditor : OdinEditor
    {
        private InspectorProperty _generateDirectoryProperty;
        private InspectorProperty _namespaceProperty;
        private InspectorProperty _autoScriptNameProperty;
        private InspectorProperty _scriptNameProperty;
        private InspectorProperty _baseClassProperty;
        private InspectorProperty _bindersGroupTypeProperty;
        private InspectorProperty _bindersGroupNameProperty;
        private InspectorProperty _buildScriptTypeProperty;
        private InspectorProperty _isInitializedProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _generateDirectoryProperty = Tree.RootProperty.Children["_generateDirectory"];
            _namespaceProperty = Tree.RootProperty.Children["_namespace"];
            _autoScriptNameProperty = Tree.RootProperty.Children["_autoScriptName"];
            _scriptNameProperty = Tree.RootProperty.Children["_scriptName"];
            _baseClassProperty = Tree.RootProperty.Children["_baseClass"];
            _bindersGroupTypeProperty = Tree.RootProperty.Children["_bindersGroupType"];
            _bindersGroupNameProperty = Tree.RootProperty.Children["_bindersGroupName"];
            _buildScriptTypeProperty = Tree.RootProperty.Children["_buildScriptType"];
            _isInitializedProperty = Tree.RootProperty.Children["_isInitialized"];
        }

        private void UnInitializeAll()
        {
            for (int i = 0; i < targets.Length; i++)
            {
                _isInitializedProperty.ValueEntry.WeakValues[i] = false;
            }
        }

        private void EnsureInitialize()
        {
            var settings = BuilderSettings.Instance;
            for (int i = 0; i < targets.Length; i++)
            {
                if (!(bool)_isInitializedProperty.ValueEntry.WeakValues[i])
                {
                    var comp = (Builder)targets[i];
                    _scriptNameProperty.ValueEntry.WeakValues[i] = comp.gameObject.name;
                    _generateDirectoryProperty.ValueEntry.WeakValues[i] = settings.Default.GenerateDir;
                    _namespaceProperty.ValueEntry.WeakValues[i] = settings.Default.Namespace;
                    _baseClassProperty.ValueEntry.WeakValues[i] = settings.Default.BaseType;
                    _bindersGroupTypeProperty.ValueEntry.WeakValues[i] = settings.Default.BindersGroupType;
                    _bindersGroupNameProperty.ValueEntry.WeakValues[i] = settings.Default.BindersGroupName;

                    _isInitializedProperty.ValueEntry.WeakValues[i] = true;
                }
            }
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw(true);

            EnsureInitialize();

            bool hasBuilded = false;
            for (int i = 0; i < targets.Length; i++)
            {
                var builder = (Builder)targets[i];

                if (builder.IsBuild())
                {
                    var classType = builder.TryGetScriptType();

                    _scriptNameProperty.ValueEntry.WeakValues[i] = classType.Name;
                    _autoScriptNameProperty.ValueEntry.WeakValues[i] = classType.Name == builder.gameObject.name;
                    _baseClassProperty.ValueEntry.WeakValues[i] = classType.BaseType;
                    _namespaceProperty.ValueEntry.WeakValues[i] = classType.Namespace;
                    var monoScript = classType.GetMonoScript();
                    var path = AssetDatabase.GetAssetPath(monoScript);
                    var dir = path[..path.LastIndexOf('/')];
                    dir = dir["Assets/".Length..];
                    _generateDirectoryProperty.ValueEntry.WeakValues[i] = dir;

                    hasBuilded = true;
                }
            }

            EasyEditorGUI.Title("代码生成设置");
            EditorGUI.BeginDisabledGroup(hasBuilded);

            _generateDirectoryProperty.DrawEx("代码生成目录");
            _namespaceProperty.DrawEx("命名空间");
            _autoScriptNameProperty.DrawEx("自动命名", "类名与游戏对象的名称相同");
            _scriptNameProperty.DrawEx("生成脚本名");

            string reallyScriptName = null;
            for (int i = 0; i < targets.Length; i++)
            {
                var builder = (Builder)targets[i];
                if (reallyScriptName == null)
                {
                    reallyScriptName = builder.GetScriptName();
                }
                else
                {
                    if (reallyScriptName != builder.GetScriptName())
                    {
                        reallyScriptName = "一";
                    }
                }
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("实际生成脚本名", reallyScriptName);
            EditorGUI.EndDisabledGroup();

            GUIContent btnLabel;
            if (_baseClassProperty.ValueEntry.WeakValues.Cast<Type>().AllSame())
            {
                var type = (Type)_baseClassProperty.ValueEntry.WeakSmartValue;

                btnLabel = type == null
                    ? EditorHelper.NoneSelectorBtnLabel
                    : EditorHelper.TempContent2(type.ToString());
            }
            else
            {
                btnLabel = EditorHelper.TempContent2("一");
            }

            EasyEditorGUI.DrawSelectorDropdown(
                () => BuilderSettings.BaseTypes,
                EditorHelper.TempContent("脚本基类"),
                btnLabel,
                t => { _baseClassProperty.ValueEntry.SetAllWeakValues(t); });

            EditorGUI.EndDisabledGroup();

            _bindersGroupTypeProperty.DrawEx("绑定器分组类型");
            _bindersGroupNameProperty.DrawEx("绑定器分组名称");

            if (!hasBuilded)
            {
                if (GUILayout.Button("恢复默认值"))
                {
                    UnInitializeAll();
                }
            }

            EasyEditorGUI.Title("代码生成");

            _buildScriptTypeProperty.DrawEx("脚本类型");

            EditorGUI.BeginDisabledGroup(!targets.Cast<Builder>().Any(CodeBuild.CanBuild));
            if (GUILayout.Button("生成脚本"))
            {
                foreach (var builder in targets.Cast<Builder>())
                {
                    CodeBuild.TryBuild(builder);
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(targets.Length != 1);
            if (GUILayout.Button("复制声明"))
            {
                
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!targets.Cast<Builder>().Any(builder => builder.IsBuild()));
            if (GUILayout.Button("绑定脚本"))
            {
                try
                {
                    foreach (var builder in targets.Cast<Builder>())
                    {
                        if (!builder.IsBuild())
                            continue;

                        var type = builder.TryGetScriptType();
                        var comp = builder.GetComponent(type);
                        if (comp == null)
                        {
                            comp = builder.gameObject.AddComponent(type);
                        }

                        builder.BindTo(comp);
                    }
                }
                catch (Exception e)
                {
                    EditorUtility.DisplayDialog("错误", $"{e.Message}", "确认");
                }
            }
            EditorGUI.EndDisabledGroup();

            Tree.EndDraw();
        }
    }
}
