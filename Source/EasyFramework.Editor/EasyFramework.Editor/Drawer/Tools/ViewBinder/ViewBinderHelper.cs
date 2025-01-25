using System.Collections.Generic;
using System.Linq;
using UnityEditor;

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

        [MenuItem("GameObject/EasyFramework/Add ViewBinder")]
        private static void AddViewBinder()
        {
            foreach (var o in Selection.gameObjects)
            {
                var c = o.GetOrAddComponent<ViewBinder>();
            }
        }
    }
}
