using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyFramework.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EasyFramework.ToolKit.Editor
{
    public static class ViewControllerEditorUtility
    {
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
            var originComp = comp;

            var classType = ReflectionUtility.FindType(cfg.Namespace, cfg.ScriptName);
            Debug.Assert(classType != null);

            if (comp.GetType() != classType)
            {
                var newComp = comp.GetComponent(classType);
                if (newComp == null)
                {
                    newComp = comp.gameObject.AddComponent(classType);
                    ((IViewController)newComp).Config = ((IViewController)comp).Config;
                }

                comp = newComp;
            }

            InternalBind((IViewController)comp);

            if (originComp != comp)
            {
                Object.DestroyImmediate(comp);
                GUIHelper.ExitGUI(true);
            }
        }

        private static void InternalBind(IViewController controller)
        {
            var binders = ViewControllerUtility.GetAllBinders(controller);

            var fields = controller.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<FromViewBinderAttribute>() != null)
                .ToArray();

            foreach (var binder in binders)
            {
                var comp = (Component)binder;
                if (binder.Config.OwnerController != null)
                {
                    Debug.Assert(binder.Config.OwnerController == controller);
                }

                var bindName = ViewBinderEditorUtility.GetBindName(binder);
                var f = fields.FirstOrDefault(f => f.Name == bindName);
                if (f == null)
                {
                    EditorUtility.DisplayDialog("错误",
                        $"绑定失败：绑定器 '{comp.gameObject.name}' 未在控制器中绑定，需要重新生成代码", "确认");
                    return;
                }

                var bindObject = ViewBinderEditorUtility.GetBindObject(binder);
                if (bindObject == null)
                {
                    EditorUtility.DisplayDialog("错误",
                        $"绑定失败：绑定器 '{comp.gameObject.name}' 没有绑定类型", "确认");
                    return;
                }

                if (ViewBinderEditorUtility.GetBindType(binder.Config.EditorConfig) != f.FieldType)
                {
                    EditorUtility.DisplayDialog("错误",
                        $"绑定失败：绑定器 '{comp.gameObject.name}' 的绑定类型与控制器中实际的类型不相同，需要重新生成代码", "确认");
                    return;
                }

                f.SetValue(controller, bindObject);
            }
        }

        public static string GetScriptName(IViewController controller)
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
