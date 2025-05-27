using System;
using System.Linq;
using EasyFramework.Core;
using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    [CustomEditor(typeof(Builder))]
    [CanEditMultipleObjects]
    public class BuilderEditor : OdinEditor
    {
        private InspectorProperty _generateDirectoryProperty;
        private InspectorProperty _namespaceProperty;
        private InspectorProperty _useGameObjectNameProperty;
        private InspectorProperty _scriptNameProperty;
        private InspectorProperty _controllerBaseClassProperty;
        private InspectorProperty _uiPanelBaseClassProperty;
        private InspectorProperty _architectureTypeProperty;
        private InspectorProperty _bindersGroupTypeProperty;
        private InspectorProperty _bindersGroupNameProperty;
        private InspectorProperty _buildScriptTypeProperty;
        private InspectorProperty _isInitializedProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _generateDirectoryProperty = Tree.RootProperty.Children["_generateDirectory"];
            _namespaceProperty = Tree.RootProperty.Children["_namespace"];
            _useGameObjectNameProperty = Tree.RootProperty.Children["_useGameObjectName"];
            _scriptNameProperty = Tree.RootProperty.Children["_scriptName"];
            _controllerBaseClassProperty = Tree.RootProperty.Children["_controllerBaseClass"];
            _uiPanelBaseClassProperty = Tree.RootProperty.Children["_uiPanelBaseClass"];
            _architectureTypeProperty = Tree.RootProperty.Children["_architectureType"];
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
                    _useGameObjectNameProperty.ValueEntry.WeakValues[i] = true;
                    _generateDirectoryProperty.ValueEntry.WeakValues[i] = settings.DefaultGenerateDirectory;
                    _namespaceProperty.ValueEntry.WeakValues[i] = settings.DefaultNamespace;
                    _controllerBaseClassProperty.ValueEntry.WeakValues[i] = settings.DefaultControllerBaseType;
                    _uiPanelBaseClassProperty.ValueEntry.WeakValues[i] = settings.DefaultUIPanelBaseType;
                    _architectureTypeProperty.ValueEntry.WeakValues[i] = settings.DefaultArchitectureType;
                    _bindersGroupTypeProperty.ValueEntry.WeakValues[i] = settings.DefaultBindersGroupType;
                    _bindersGroupNameProperty.ValueEntry.WeakValues[i] = settings.DefaultBindersGroupName;

                    _isInitializedProperty.ValueEntry.WeakValues[i] = true;
                }
            }
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw(true);

            EnsureInitialize();

            _generateDirectoryProperty.State.Expanded = EasyEditorGUI.FoldoutGroup(
                _generateDirectoryProperty,
                "代码生成配置",
                _generateDirectoryProperty.State.Expanded,
                DrawSettings);

            EasyEditorGUI.Title("代码生成操作");

            EditorGUI.BeginDisabledGroup(!targets.Cast<Builder>().Any(CodeBuild.CanBuild));
            if (GUILayout.Button("生成脚本"))
            {
                foreach (var builder in targets.Cast<Builder>())
                {
                    try
                    {
                        CodeBuild.Build(builder);
                    }
                    catch (Exception e)
                    {
                        EditorUtility.DisplayDialog("错误", $"生成脚本失败：{e.Message}", "确认");
                    }
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(targets.Length != 1);
            if (GUILayout.Button("复制字段声明代码"))
            {
                EditorGUIUtility.systemCopyBuffer = CodeBuild.GetIndentedBinderFieldsCode((Builder)target);
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
                        ComponentUtility.MoveComponentDown(builder);
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

        private void DrawSettings(Rect headerRect)
        {
            EasyEditorGUI.DrawSelectorDropdown(
                () => BuilderSettings.Instance.GenerateDirectoryPresets,
                EditorHelper.TempContent("代码生成目录"),
                _generateDirectoryProperty.GetSmartContent(),
                t => { _generateDirectoryProperty.ValueEntry.SetAllWeakValues(t); });
            
            EasyEditorGUI.DrawSelectorDropdown(
                () => BuilderSettings.Instance.NamespacePresets,
                EditorHelper.TempContent("命名空间"),
                _namespaceProperty.GetSmartContent(),
                t => { _namespaceProperty.ValueEntry.SetAllWeakValues(t); });

            _useGameObjectNameProperty.DrawEx("使用游戏对象名称", "脚本名直接使用游戏对象名称");
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
            
            _buildScriptTypeProperty.DrawEx("脚本类型");

            if (_buildScriptTypeProperty.ValueEntry.WeakValues.Cast<Builder.ScriptType>().AllSame())
            {
                var scriptType = _buildScriptTypeProperty.ValueEntry.WeakSmartValueT<Builder.ScriptType>();
                InspectorProperty targetBaseClassProperty;
                switch (scriptType)
                {
                    case Builder.ScriptType.UIPanel:
                        targetBaseClassProperty = _uiPanelBaseClassProperty;
                        break;
                    case Builder.ScriptType.Controller:
                        targetBaseClassProperty = _controllerBaseClassProperty;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                EasyEditorGUI.DrawSelectorDropdown(
                    () => BuilderSettings.BaseTypes,
                    EditorHelper.TempContent("脚本基类"),
                    targetBaseClassProperty.GetSmartContent(),
                    t => { targetBaseClassProperty.ValueEntry.SetAllWeakValues(t); });
            }
            else
            {
                EditorGUILayout.LabelField("脚本基类", "脚本类型冲突，无法选择");
            }

            EasyEditorGUI.DrawSelectorDropdown(
                () => BuilderSettings.ArchitectureTypes,
                EditorHelper.TempContent("架构"),
                _architectureTypeProperty.GetSmartContent(),
                t => { _architectureTypeProperty.ValueEntry.SetAllWeakValues(t); });


            _bindersGroupTypeProperty.DrawEx("绑定器分组类型");
            _bindersGroupNameProperty.DrawEx("绑定器分组名称");

            if (GUILayout.Button(EditorHelper.TempContent("自动赋值", "查找该游戏对象上继承了IController的组件，然后应用该组件的脚本信息。")))
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    var builder = (Builder)targets[i];
                    var ctrl = builder.GetComponent(typeof(IController));
                    if (ctrl == null)
                        continue;

                    var classType = ctrl.GetType();
                    _scriptNameProperty.ValueEntry.WeakValues[i] = classType.Name;
                    _useGameObjectNameProperty.ValueEntry.WeakValues[i] = classType.Name == builder.gameObject.name;

                    if (classType.HasInterface(typeof(IController)))
                    {
                        _buildScriptTypeProperty.ValueEntry.WeakValues[i] = Builder.ScriptType.Controller;
                        _controllerBaseClassProperty.ValueEntry.WeakValues[i] = classType.BaseType;
                    }

                    _namespaceProperty.ValueEntry.WeakValues[i] = classType.Namespace;
                    var monoScript = classType.GetMonoScript();
                    var path = AssetDatabase.GetAssetPath(monoScript);
                    var dir = path[..path.LastIndexOf('/')];
                    dir = dir["Assets/".Length..];
                    _generateDirectoryProperty.ValueEntry.WeakValues[i] = dir;
                }
            }

            if (GUILayout.Button("恢复默认值"))
            {
                UnInitializeAll();
            }
        }
    }
}
