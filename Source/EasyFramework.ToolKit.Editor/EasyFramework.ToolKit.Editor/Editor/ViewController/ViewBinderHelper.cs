using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EasyFramework.ToolKit.Editor
{
    public static class ViewBinderHelper
    {
        public static List<string> GetCommentSplits(ViewBinderEditorConfig config)
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

        public static Component[] GetCandidateComponents(Component component)
        {
            var priorityList = ViewBinderSettings.Instance.Priority;

            var components = component.GetComponents<Component>()
                .Where(c => c != null)
                .ToList();

            components.Sort((a, b) =>
            {
                var indexA = priorityList.IndexOf(a.GetType(), false, true);
                var indexB = priorityList.IndexOf(b.GetType(), false, true);

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
            return components.ToArray();
        }
        
        public static Object GetBindObject(IViewBinder binder)
        {
            var cfg = binder.Config.EditorConfig;
            var comp = (Component)binder;
            if (cfg.BindGameObject)
            {
                return comp.gameObject;
            }

            return comp.GetComponent(cfg.BindComponentType);
        }

        public static Type GetBindType(ViewBinderEditorConfig config)
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

        public static string GetBindName(IViewBinder binder)
        {
            var cfg = binder.Config.EditorConfig;
            var comp = (Component)binder;
            if (cfg.AutoBindName)
            {
                cfg.BindName = comp.gameObject.name;
            }

            if (cfg.ProcessBindName)
            {
                return ProcessName(cfg.BindName, cfg.BindAccess);
            }

            return cfg.BindName;
        }

        [MenuItem("GameObject/EasyFramework/添加 ViewBinder")]
        private static void AddViewBinder()
        {
            foreach (var o in Selection.gameObjects)
            {
                var binder = o.GetOrAddComponent<ViewBinder>();
            }
        }
    }
}
