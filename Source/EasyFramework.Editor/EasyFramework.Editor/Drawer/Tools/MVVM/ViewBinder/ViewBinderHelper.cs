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
            var parents = component.transform.FindParents(p =>
                p.gameObject.HasComponent<IViewModel>()).ToList();

            binder.Info ??= new ViewBinderInfo();
            binder.Info.EditorData ??= new SerializedAny();

            var editorInfo = binder.Info.EditorData.Get<ViewBinderEditorInfo>() ?? new ViewBinderEditorInfo();
            var settings = ViewBinderSettings.Instance;
            
            if (!editorInfo.IsInitialized)
            {
                var candidateComponents = component.GetComponents<Component>()
                    .Where(c => c != null)
                    .ToArray();

                Component initialComponent;
                if (candidateComponents.Length > 1)
                {
                    var t = candidateComponents[1];
                    if (t.GetType() == typeof(ViewBinder))
                    {
                        initialComponent = candidateComponents.Length > 2
                            ? candidateComponents[2]
                            : candidateComponents[0];
                    }
                    else
                    {
                        initialComponent = t;
                    }
                }
                else
                {
                    initialComponent = candidateComponents[0];
                }

                binder.Info.BindComponent = initialComponent;

                if (parents.IsNotNullOrEmpty())
                {
                    binder.Info.OwnerViewModel = parents[0];
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
