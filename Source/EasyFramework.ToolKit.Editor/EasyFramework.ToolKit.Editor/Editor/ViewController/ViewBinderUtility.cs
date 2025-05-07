using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using EasyFramework.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EasyFramework.ToolKit.Editor
{
    public static class ViewBinderUtility
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

        public static Type[] GetSpecficableBindTypes(Type type)
        {
            if (type == null)
                return new Type[]{};
            return type.GetAllBaseTypes(true, true)
                .Where(t => !t.IsInterface && t.IsSubclassOf(typeof(Object)))
                .ToArray();
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
            var settings = ViewBinderSettings.Instance;

            Array.Sort(types, (a, b) =>
            {
                var indexA = settings.IndexByPriorityOf(a, false, true);
                var indexB = settings.IndexByPriorityOf(b, false, true);

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
            var settings = ViewBinderSettings.Instance;

            foreach (var type in types)
            {
                if (settings.Priorities.Any(priority => priority == type))
                {
                    return type;
                }
            }

            return types[0];
        }

        public static Type[] GetBindableComponentTypes(this IViewBinder binder)
        {
            var comp = (Component)binder;
            var types = comp.GetComponents<Component>()
                .Where(c => c != null)
                .Select(c => c.GetType())
                .Distinct()
                .ToArray();
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

        public static Component[] GetOwners(this IViewBinder binder)
        {
            var comp = (Component)binder;
            return comp.gameObject.GetComponentsInParent(typeof(IViewController), true);
        }

        public static void UseDefault(this IViewBinder binder)
        {
            var settings = ViewBinderSettings.Instance;

            var comp = (Component)binder;
            var editorConfig = binder.Config.EditorConfig;

            var owners = binder.GetOwners();
            if (owners.Length > 0)
            {
                var o = owners[0];
                // 如果游戏对象相同，说明是既持有Controller又持有Binder
                // 绑定再上一级的Controller(如果有)
                if (o.gameObject == comp.gameObject)
                {
                    if (owners.Length > 1)
                        o = owners[1];
                }

                binder.Config.OwnerController = o;
            }
            else
            {
                binder.Config.OwnerController = null;
            }

            editorConfig.BindName = comp.gameObject.name;
            var bindableComps = ((IViewBinder)comp).GetBindableComponentTypes();
            editorConfig.BindComponentType = bindableComps[0];

            editorConfig.SpecificBindType = GetDefaultSpecialType(
                GetSpecficableBindTypes(editorConfig.BindComponentType));

            editorConfig.BindGameObject = settings.Default.BindGameObject;
            editorConfig.BindAccess = settings.Default.BindAccess;
            editorConfig.AutoBindName = settings.Default.AutoBindName;
            editorConfig.ProcessBindName = settings.Default.ProcessBindName;
            editorConfig.UseDocumentComment = settings.Default.UseDocumentComment;
            editorConfig.AutoAddParaToComment = settings.Default.AutoAddParaToComment;
            editorConfig.Comment = settings.Default.Comment;
        }

        [MenuItem("GameObject/EasyFramework/Add ViewBinder", false)]
        private static void AddViewBinder()
        {
            foreach (var o in Selection.gameObjects)
            {
                if (o.GetComponent<IViewBinder>() != null)
                    continue;
                o.AddComponent<ViewBinder>();
                EditorUtility.SetDirty(o);
                EditorSceneManager.MarkSceneDirty(o.scene);
            }
        }
    }
}
