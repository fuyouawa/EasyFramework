using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EasyFramework.ToolKit.Editor
{
    public static class ViewBinderEditorUtility
    {
        private static List<string> GetCommentSplits(ViewBinderEditorConfig config)
        {
            if (config.Comment.IsNullOrWhiteSpace())
            {
                return null;
            }

            var comment = config.Comment.Replace("\r\n", "\n");
            var commentSplits = comment.Split('\n').ToList();

            if (config.AutoAddParaToComment)
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

        public static string GetComment(this ViewBinderEditorConfig config)
        {
            var splits = GetCommentSplits(config);
            if (splits.IsNullOrEmpty())
                return string.Empty;
            return string.Join("\n", splits);
        }

        public static void SortTypesByPriorities(Type[] types)
        {
            var priorities = ViewBinderSettings.Instance.Priorities;

            Array.Sort(types, (a, b) =>
            {
                var indexA = priorities.MatchIndexOf(a, false, true);
                var indexB = priorities.MatchIndexOf(b, false, true);

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
        }

        public static Type GetDefaultSpecialType(Type[] types)
        {
            var priorities = ViewBinderSettings.Instance.Priorities;

            foreach (var type in types)
            {
                if (priorities.Collection.Any(priority => priority.Value == type))
                {
                    return type;
                }
            }
            return types[0];
        }

        public static Type[] GetBindableComponentTypes(this IViewBinder binder)
        {
            var types = ViewBinderUtility.GetBindableComponentTypes(binder);
            SortTypesByPriorities(types);
            return types;
        }
        
        public static Object GetBindObject(this IViewBinder binder)
        {
            var cfg = binder.Config.EditorConfig;
            var comp = (Component)binder;
            if (cfg.BindGameObject)
            {
                return comp.gameObject;
            }

            return comp.GetComponent(cfg.BindComponentType);
        }

        public static Type GetBindType(this ViewBinderEditorConfig config)
        {
            if (config.BindGameObject)
            {
                return typeof(GameObject);
            }

            return config.SpecificBindType;
        }

        private static string ProcessName(string name, ViewBindAccess access)
        {
            if (name.IsNullOrWhiteSpace())
                return name;

            //TODO 更多情况的处理
            if (access == ViewBindAccess.Public)
            {
                return char.ToUpper(name[0]) + name[1..];
            }

            name = char.ToLower(name[0]) + name[1..];
            return '_' + name;
        }

        public static string GetBindName(this IViewBinder binder)
        {
            var cfg = binder.Config.EditorConfig;
            var comp = (Component)binder;

            var name = cfg.BindName;

            if (cfg.AutoBindName)
            {
                name = comp.gameObject.name;
            }

            if (cfg.ProcessBindName)
            {
                return ProcessName(name, cfg.BindAccess);
            }

            return name;
        }

        [MenuItem("GameObject/EasyFramework/Add ViewBinder")]
        private static void AddViewBinder()
        {
            foreach (var o in Selection.gameObjects)
            {
                var binder = o.GetOrAddComponent<ViewBinder>();
            }
        }
    }
}
