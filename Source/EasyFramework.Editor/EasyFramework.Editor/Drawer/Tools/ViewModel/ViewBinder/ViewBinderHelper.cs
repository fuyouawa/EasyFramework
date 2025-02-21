using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor.Drawer
{
    public static class ViewBinderHelper
    {
        public static List<string> GetCommentSplits(ViewBinderEditorInfo editorInfo)
        {
            if (editorInfo.Comment.IsNullOrWhiteSpace())
            {
                return null;
            }

            var comment = editorInfo.Comment.Replace("\r\n", "\n");
            var commentSplits = comment.Split('\n').ToList();

            if (editorInfo.AutoAddCommentPara)
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

        public static void InitializeBinder(IViewBinder binder)
        {
            var component = (Component)binder;

            binder.Info ??= new ViewBinderInfo();
            binder.Info.EditorData ??= new SerializedAny();

            var editorInfo = binder.Info.EditorData.Get<ViewBinderEditorInfo>() ?? new ViewBinderEditorInfo();
            var settings = ViewBinderSettings.Instance;

            if (!editorInfo.IsInitialized)
            {
                var candidateComponents = GetCandidateComponents(component);

                var initialComponent = candidateComponents[0];

                editorInfo.BindComponent = initialComponent;

                var parents = component.transform.FindObjectsByTypeInParents<IViewModel>(true);
                if (parents.IsNotNullOrEmpty())
                {
                    var comp = (Component)parents[0];
                    binder.Info.Owner = comp.transform;
                }

                editorInfo.Name = component.gameObject.name;
                editorInfo.Access = settings.Access;
                editorInfo.AutoNamingNotations = settings.AutoNamingNotations;
                editorInfo.AutoAddCommentPara = settings.AutoAddCommentPara;
                editorInfo.Comment = settings.Comment;

                editorInfo.IsInitialized = true;

                binder.Info.EditorData.Set(editorInfo);
                EditorUtility.SetDirty(component);
            }
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

        [MenuItem("GameObject/EasyFramework/添加 ViewBinder")]
        private static void AddViewBinder()
        {
            foreach (var o in Selection.gameObjects)
            {
                var binder = o.GetOrAddComponent<ViewBinder>();

                ViewBinderHelper.InitializeBinder(binder);
            }
        }
    }
}
