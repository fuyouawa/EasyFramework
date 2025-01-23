using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using UnityEngine;
using EasyFramework.Inspector;
using UnityEditor;

namespace EasyFramework.Editor
{
    internal static class EasyControlHelper
    {
        private static FieldInfo GetArgsField(Type targetType)
        {
            return targetType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(f =>
                {
                    if (f.FieldType != typeof(EasyControlEditorArgs))
                    {
                        return false;
                    }

                    return f.IsPublic || f.HasCustomAttribute<SerializeField>();
                });
        }

        public static EasyControlEditorArgs GetArgs(object target)
        {
            return GetArgsField(target.GetType())?.GetValue(target) as EasyControlEditorArgs;
        }

        public static void SetArgs(object target, EasyControlEditorArgs args)
        {
            GetArgsField(target.GetType())?.SetValue(target, args);
        }

        private static Type[] _easyControlTypes;

        public static bool IsEasyControl(Type targetType)
        {
            if (_easyControlTypes == null)
            {
                _easyControlTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => GetArgsField(t) != null).ToArray();
            }

            return Array.Exists(_easyControlTypes, type => targetType == type);
        }

        public static EasyControlEditorArgs GetArgsInGameObject(GameObject target)
        {
            foreach (var component in target.GetComponents<Component>())
            {
                if (component == null)
                    continue;
                var args = GetArgs(component);
                if (args != null)
                {
                    return args;
                }
            }

            return null;
        }

        public static List<Component> GetChildren(GameObject target)
        {
            return target.GetComponentsInChildren<Component>()
                .Where(c =>
                {
                    if (c == null)
                        return false;
                    if (c.gameObject == target)
                        return false;
                    var args = GetArgs(c);
                    if (args == null)
                        return false;
                    return args.DoBounder;
                })
                .ToList();
        }
        
        public static void CheckIdentifier(string name, string id)
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
        
        public static List<string> GetBounderCommentSplits(EasyControlEditorArgs args)
        {
            if (args.Bounder.Comment.IsNullOrWhiteSpace())
            {
                return null;
            }

            var comment = args.Bounder.Comment.Replace("\r\n", "\n");
            var commentSplits = comment.Split('\n').ToList();

            if (args.Bounder.AutoAddCommentPara)
            {
                for (int i = 0; i < commentSplits.Count; i++)
                {
                    commentSplits[i] = "<para>" + commentSplits[i] + "</para>";
                }
            }

            commentSplits.Insert(0, "<summary>");
            commentSplits.Add("</summary>");

            return commentSplits;
        }

        [MenuItem("GameObject/EasyFramework/添加EasyControl视图模型")]
        private static void AddEasyControlViewModel()
        {
            foreach (var o in Selection.gameObjects)
            {
                var c = o.GetOrAddComponent<EasyControl>();
                EasyControlHelper.GetArgs(c).DoViewModel = true;
            }
        }

        [MenuItem("GameObject/EasyFramework/添加EasyControl绑定")]
        private static void AddEasyControlBounder()
        {
            foreach (var o in Selection.gameObjects)
            {
                var c = o.GetOrAddComponent<EasyControl>();
                EasyControlHelper.GetArgs(c).DoBounder = true;
            }
        }
    }
}
