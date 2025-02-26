using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EasyFramework.Editor.Drawer
{
    public static class IViewBinderExtension
    {
        public static Object GetBindObject(this IViewBinder binder)
        {
            var editorInfo = binder.Info.EditorData.Get<ViewBinderEditorInfo>()!;

            if (editorInfo.BindGameObject)
            {
                var comp = (Component)binder;
                return comp.gameObject;
            }
            else
            {
                return editorInfo.BindComponent;
            }
        }

        public static Type GetBindType(this IViewBinder binder)
        {
            var editorInfo = binder.Info.EditorData.Get<ViewBinderEditorInfo>()!;

            if (editorInfo.BindGameObject)
            {
                return typeof(GameObject);
            }
            else
            {
                return editorInfo.BindType;
            }
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
            var editorInfo = binder.Info.EditorData.Get<ViewBinderEditorInfo>()!;
            var comp = (Component)binder;
            if (editorInfo.NameSameAsGameObjectName)
            {
                editorInfo.Name = comp.gameObject.name;
            }

            if (editorInfo.AutoNamingNotations)
            {
                return ProcessName(editorInfo.Name, editorInfo.Access);
            }

            return editorInfo.Name;
        }
    }
}
