using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EasyFramework.ToolKit.Editor
{
    public static class ViewControllerUtility
    {
        private static Type[] s_baseTypes;

        public static Type[] BaseTypes
        {
            get
            {
                s_baseTypes ??= AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(asm => asm.GetTypes())
                    .Where(t => t.IsSubclassOf(typeof(Component)) && !t.ContainsGenericParameters && t.IsPublic)
                    .ToArray();
                return s_baseTypes;
            }
        }

        public static void CheckIdentifierWithMessage(string name, string id)
        {
            var error = GetIdentifierError(name, id);
            if (error.IsNotNullOrEmpty())
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
            }
        }

        public static string GetIdentifierError(string name, string id)
        {
            try
            {
                id.IsValidIdentifier(true);

                return string.Empty;
            }
            catch (InvalidIdentifierException e)
            {
                return e.Type switch
                {
                    InvalidIdentifierTypes.Empty => $"{name}为空",
                    InvalidIdentifierTypes.IllegalBegin => $"{name}必须以字母或下划线开头",
                    InvalidIdentifierTypes.IllegalContent => $"{name}的其余部分只能是字母、数字或下划线",
                    InvalidIdentifierTypes.CSharpKeywordConflict => $"{name}不能与C#关键字冲突",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public static void Bind(IViewController controller)
        {
            var cfg = controller.Config.EditorConfig;
            var comp = (Component)controller;
            var ctrl = (IViewController)comp;
            var originComp = comp;

            var classType = ReflectionUtility.FindType(cfg.Namespace, cfg.ScriptName);
            Debug.Assert(classType != null);

            if (comp.GetType() != classType)
            {
                var newComp = comp.GetComponent(classType);
                if (newComp == null)
                {
                    newComp = comp.gameObject.AddComponent(classType);
                    var newCtrl = ((IViewController)newComp);
                    newCtrl.Config = ctrl.Config;
                    newCtrl.Config.EditorConfig.IsJustBound = true;
                }

                comp = newComp;
            }

            var binders = controller.GetAllBinders();
            foreach (var binder in binders)
            {
                binder.Config.OwnerController = comp;
            }

            InternalBind(ctrl);

            EditorUtility.SetDirty(comp);
            if (originComp != comp)
            {
                Object.DestroyImmediate(originComp);
                GUIHelper.ExitGUI(true);
            }
        }

        private static void InternalBind(IViewController controller)
        {
            var binders = controller.GetAllBinders();

            var fields = controller.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<AutoBindingAttribute>() != null)
                .ToArray();

            foreach (var binder in binders)
            {
                var comp = (Component)binder;
                if (binder.Config.OwnerController != null)
                {
                    Debug.Assert(binder.Config.OwnerController == (Component)controller);
                }

                var bindName = binder.GetBindName();
                var f = fields.FirstOrDefault(f => f.Name == bindName);
                if (f == null)
                {
                    EditorUtility.DisplayDialog("错误",
                        $"绑定失败：绑定器 '{comp.gameObject.name}' 未在控制器中绑定，需要重新生成代码", "确认");
                    return;
                }

                var bindObject = binder.GetBindObject();
                if (bindObject == null)
                {
                    EditorUtility.DisplayDialog("错误",
                        $"绑定失败：绑定器 '{comp.gameObject.name}' 没有绑定类型", "确认");
                    return;
                }

                if (binder.Config.EditorConfig.GetBindType() != f.FieldType)
                {
                    EditorUtility.DisplayDialog("错误",
                        $"绑定失败：绑定器 '{comp.gameObject.name}' 的绑定类型与控制器中实际的类型不相同，需要重新生成代码", "确认");
                    return;
                }

                f.SetValue(controller, bindObject);
            }
        }

        public static string GetScriptName(this IViewController controller)
        {
            var cfg = controller.Config.EditorConfig;
            var comp = (Component)controller;
            var name = cfg.ScriptName;

            if (cfg.AutoScriptName)
            {
                name = comp.gameObject.name;
            }

            return name;
        }

        public static List<IViewBinder> GetOtherBinders(this IViewController controller)
        {
            return controller.Config.EditorConfig.OtherBindersList
                .SelectMany(b => b.Targets.Collection)
                .Where(c => c != null && c.Binder != null)
                .Select(c => (IViewBinder)c.Binder)
                .ToList();
        }

        public static List<IViewBinder> GetAllBinders(this IViewController controller)
        {
            var total = new List<IViewBinder>();

            total.AddRange(GetChildrenBinders(controller));
            total.AddRange(GetOtherBinders(controller));

            return total;
        }

        public static List<IViewBinder> GetChildrenBinders(this IViewController controller)
        {
            var comp = (Component)controller;
            return comp.transform.GetComponentsInChildren<IViewBinder>(true)
                .Where(b => b.Config.OwnerController == comp)
                .ToList();
        }

        public static void UseDefault(this IViewController controller)
        {
            var settings = ViewControllerSettings.Instance;

            var comp = (Component)controller;
            var cfg = controller.Config.EditorConfig;

            cfg.ScriptName = comp.gameObject.name;
            cfg.GenerateDir = settings.Default.GenerateDir;
            cfg.Namespace = settings.Default.Namespace;
            cfg.BaseClass = settings.Default.BaseType;
            cfg.BindersGroupType = settings.Default.BindersGroupType;
            cfg.BindersGroupName = settings.Default.BindersGroupName;
        }


        [MenuItem("GameObject/EasyFramework/Add ViewController")]
        private static void AddViewController()
        {
            foreach (var o in Selection.gameObjects)
            {
                var c = o.GetOrAddComponent<ViewController>();
            }
        }
    }
}
