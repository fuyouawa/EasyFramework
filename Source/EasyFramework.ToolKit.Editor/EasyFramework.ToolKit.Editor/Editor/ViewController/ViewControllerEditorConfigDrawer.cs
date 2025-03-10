using System;
using System.IO;
using EasyFramework.Editor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class ViewControllerEditorConfigDrawer : OdinValueDrawer<ViewControllerEditorConfig>
    {
        private InspectorProperty _propertyOfOtherBindersList;

        protected override void Initialize()
        {
            _propertyOfOtherBindersList = Property.Children[nameof(ViewControllerEditorConfig.OtherBindersList)];
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var val = ValueEntry.SmartValue;
            var comp = GetTargetComponent(Property);
            var ctrl = (IViewController)comp;

            var isBuildAndBind = comp as ViewController == null;

            var classType = isBuildAndBind
                ? comp.GetType()
                : ReflectionUtility.FindType(val.Namespace, val.ScriptName);

            var isBuild = classType != null && classType != typeof(ViewController);

            if (isBuild)
            {
                val.ScriptName = classType.Name;
                val.AutoScriptName = val.ScriptName == comp.gameObject.name;
                val.BaseClass = classType.BaseType;
                val.Namespace = classType.Namespace;
                var monoScript = classType.GetMonoScript();
                var path = AssetDatabase.GetAssetPath(monoScript);
                var dir = path[..path.LastIndexOf('/')];
                dir = dir["Assets/".Length..];
                val.GenerateDir = dir;
            }

            EasyEditorGUI.Title("代码生成设置");

            EditorGUI.BeginDisabledGroup(isBuild);

            EditorGUI.BeginChangeCheck();
            val.GenerateDir = SirenixEditorFields.FolderPathField(
                EditorHelper.TempContent("代码生成目录"), val.GenerateDir, "Assets", false,
                false);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(comp);
                GUIHelper.ExitGUI(false);
            }
            
            bool hasChange = false;
            EditorGUI.BeginChangeCheck();

            val.Namespace = EditorGUILayout.TextField("命名空间", val.Namespace);
            val.AutoScriptName = EditorGUILayout.Toggle(
                EditorHelper.TempContent("自动命名", "类名与游戏对象的名称相同"),
                val.AutoScriptName);
            
            val.ScriptName = EditorGUILayout.TextField("生成脚本名", val.ScriptName);
            if (EditorGUI.EndChangeCheck())
            {
                hasChange = true;
            }

            if (!isBuild && val.AutoScriptName)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("实际生成脚本名", ctrl.GetScriptName());
                EditorGUI.EndDisabledGroup();
            }

            var btnLabel = val.BaseClass == null
                ? EditorHelper.NoneSelectorBtnLabel
                : EditorHelper.TempContent2(val.BaseClass.GetNiceName());
            
            EditorGUI.BeginChangeCheck();

            EasyEditorGUI.DrawSelectorDropdown(
                ViewControllerUtility.BaseTypes,
                EditorHelper.TempContent("脚本基类"),
                btnLabel,
                type => val.BaseClass = type,
                type => type.GetNiceName());

            if (GUILayout.Button("恢复默认值"))
            {
                ((IViewController)comp).UseDefault();
                hasChange = true;
            }

            EditorGUI.EndDisabledGroup();
            
            val.BindersGroupType = EnumSelector<ViewControllerBindersGroupType>.DrawEnumField(
                EditorHelper.TempContent("绑定器分组类型"),
                val.BindersGroupType);
            if (val.BindersGroupType != ViewControllerBindersGroupType.None)
            {
                if (val.BindersGroupName.IsNullOrWhiteSpace())
                {
                    EasyEditorGUI.MessageBox("绑定器分组名称不能为空！", MessageType.Error);
                }
                val.BindersGroupName = EditorGUILayout.TextField("绑定器分组名称", val.BindersGroupName);
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                hasChange = true;
            }

            EasyEditorGUI.Title("扩展设置");

            _propertyOfOtherBindersList.Draw(EditorHelper.TempContent("其他绑定列表"));

            if (GUILayout.Button("添加其他绑定"))
            {
            }

            EasyEditorGUI.Title("代码生成操作");

            var lbl = isBuildAndBind ? "已构建" : "未构建";
            if (!isBuildAndBind)
            {
                if (isBuild)
                {
                    lbl = "已构建但未绑定";
                }
            }

            EditorGUILayout.LabelField("状态", lbl);

            EditorGUILayout.BeginHorizontal();
            var height = EditorGUIUtility.singleLineHeight;
            if (SirenixEditorGUI.SDFIconButton("生成代码", height, SdfIconType.PencilFill))
            {
                var builder = new ViewControllerBuilder(ctrl);
                if (builder.Check())
                {
                    builder.Build();
                }
            }

            EditorGUI.BeginDisabledGroup(!isBuild);
            if (SirenixEditorGUI.SDFIconButton("绑定脚本", height, SdfIconType.Bezier))
            {
                ViewControllerUtility.Bind(ctrl);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

            if (hasChange)
            {
                EditorUtility.SetDirty(comp);
            }
        }

        private static Component GetTargetComponent(InspectorProperty property)
        {
            return property.Parent.Parent.WeakSmartValue<Component>();
        }
    }
}
