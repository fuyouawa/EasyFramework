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
        private ViewControllerSettings _settings;
        private InspectorProperty _propertyOfGenerateDir;
        private InspectorProperty _propertyOfNamespace;
        private InspectorProperty _propertyOfAutoScriptName;
        private InspectorProperty _propertyOfScriptName;
        private InspectorProperty _propertyOfBaseClass;
        private InspectorProperty _propertyOfOtherBindersList;

        protected override void Initialize()
        {
            _settings = ViewControllerSettings.Instance;
            _propertyOfGenerateDir = Property.Children[nameof(ViewControllerEditorConfig.GenerateDir)];
            _propertyOfNamespace = Property.Children[nameof(ViewControllerEditorConfig.Namespace)];
            _propertyOfAutoScriptName = Property.Children[nameof(ViewControllerEditorConfig.AutoScriptName)];
            _propertyOfScriptName = Property.Children[nameof(ViewControllerEditorConfig.ScriptName)];
            _propertyOfBaseClass = Property.Children[nameof(ViewControllerEditorConfig.BaseClass)];
            _propertyOfOtherBindersList = Property.Children[nameof(ViewControllerEditorConfig.OtherBindersList)];
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            foreach (var component in Property.Components)
            {
                EnsureInitialize(component.Property);
            }

            var val = ValueEntry.SmartValue;
            var comp = GetTargetComponent(Property);

            var isBuildAndBind = comp as ViewController == null;

            var classType = ReflectionUtility.FindType(val.Namespace, val.ScriptName);
            var isBuild = classType != null && classType != typeof(ViewController);

            if (isBuildAndBind)
            {
                val.ScriptName = comp.GetType().Name;
                val.AutoScriptName = val.ScriptName == comp.gameObject.name;
            }

            EasyEditorGUI.Title("代码生成设置");

            EditorGUI.BeginDisabledGroup(isBuildAndBind);
            
            _propertyOfGenerateDir.Draw(EditorHelper.TempContent("代码生成目录"));
            _propertyOfNamespace.Draw(EditorHelper.TempContent("命名空间"));
            _propertyOfAutoScriptName.Draw(EditorHelper.TempContent("自动命名", "类名与游戏对象的名称相同"));
            _propertyOfScriptName.Draw(EditorHelper.TempContent("生成脚本名"));

            if (!isBuild && val.AutoScriptName)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("实际生成脚本名", ViewControllerEditorUtility.GetScriptName((IViewController)comp));
                EditorGUI.EndDisabledGroup();
            }
            _propertyOfBaseClass.Draw(EditorHelper.TempContent("脚本基类"));

            if (GUILayout.Button("恢复默认值"))
            {
                UseDefault(Property);
            }

            EditorGUI.EndDisabledGroup();

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
            }

            if (SirenixEditorGUI.SDFIconButton("绑定脚本", height, SdfIconType.Bezier))
            {
            }

            EditorGUILayout.EndHorizontal();
        }

        private void EnsureInitialize(InspectorProperty property)
        {
            var comp = GetTargetComponent(property);
            var cfg = property.WeakSmartValue<ViewControllerEditorConfig>();
            cfg.SetTargetComponent(comp);

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
            var cfg = property.WeakSmartValue<ViewControllerEditorConfig>();
            
            cfg.ScriptName = comp.gameObject.name;
            cfg.GenerateDir = _settings.Default.GenerateDir;
            cfg.Namespace = _settings.Default.Namespace;
            cfg.BaseClass = _settings.Default.BaseType;
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
