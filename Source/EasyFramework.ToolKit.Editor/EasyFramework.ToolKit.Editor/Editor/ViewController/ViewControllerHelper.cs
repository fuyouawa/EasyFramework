using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyFramework.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EasyFramework.ToolKit.Editor
{
    public class GenericViewBinderConfig
    {
        public GameObject Target { get; }
        public ViewBinderEditorConfig EditorConfig { get; }

        public GenericViewBinderConfig(GameObject target, ViewBinderEditorConfig editorConfig)
        {
            Target = target;
            EditorConfig = editorConfig;
        }
    }

    public static class ViewControllerHelper
    {
        private static List<Type> s_baseTypes;

        public static List<Type> BaseTypes
        {
            get
            {
                if (s_baseTypes == null)
                {
                    s_baseTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                        .Where(t => t.IsSubclassOf(typeof(Component)) && !t.IsSealed).ToList();
                }

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

        public static List<GenericViewBinderConfig> GetOtherBinderConfigs(IViewController controller)
        {
            return controller.Config.EditorConfig.OtherBindersList
                .SelectMany(b => b.Configs.Collection)
                .Select(c => new GenericViewBinderConfig(c.Target, c.EditorConfig))
                .ToList();
        }

        public static List<GenericViewBinderConfig> GetAllBinderConfigs(IViewController controller)
        {
            var total = new List<GenericViewBinderConfig>();
            var children = GetChildren(controller);
            total.AddRange(children.Select(c =>
                new GenericViewBinderConfig(((Component)c).gameObject, c.Config.EditorConfig)));

            total.AddRange(GetOtherBinderConfigs(controller));

            return total;
        }

        public static  IViewBinder[] GetChildren(IViewController controller)
        {
            var comp = (Component)controller;
            return comp.transform.GetComponentsInChildren<IViewBinder>(true)
                .Where(b => b.Config.OwnerController == controller)
                .ToArray();
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
            var children = GetChildren(controller);

            var fields = controller.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<FromViewBinderAttribute>() != null)
                .ToArray();

            foreach (var child in children)
            {
                var comp = (Component)child;
                var bindName = ViewBinderHelper.GetBindName(child);
                var f = fields.FirstOrDefault(f => f.Name == bindName);
                if (f == null)
                {
                    Debug.LogWarning($"绑定器 '{comp.gameObject.name}' 未在控制器中绑定，需要重新生成代码");
                    continue;
                }

                var bindObject = ViewBinderHelper.GetBindObject(child);
                if (bindObject == null)
                {
                    EditorUtility.DisplayDialog("错误",
                        $"绑定失败：绑定器 '{comp.gameObject.name}' 没有绑定类型", "确认");
                    return;
                }

                if (ViewBinderHelper.GetBindType(child.Config.EditorConfig) != f.FieldType)
                {
                    EditorUtility.DisplayDialog("错误",
                        $"绑定失败：绑定器 '{comp.gameObject.name}' 的绑定类型与控制器中实际的类型不相同，需要重新生成代码", "确认");
                    return;
                }

                f.SetValue(controller, bindObject);
            }


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
