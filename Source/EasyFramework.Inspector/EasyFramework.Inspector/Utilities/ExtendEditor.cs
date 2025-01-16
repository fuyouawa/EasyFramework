using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EasyGameFramework.Editor
{
    public abstract class ExtendEditor : UnityEditor.Editor
    {
        protected UnityEditor.Editor DefaultEditor;

        protected abstract string EditorTypeName { get; }

        protected virtual void OnEnable()
        {
            DefaultEditor = CreateEditor(targets, Type.GetType(EditorTypeName));
            
            var onEnable = DefaultEditor.GetType().GetMethod("OnEnable",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            onEnable?.Invoke(DefaultEditor, null);
        }

        protected virtual void OnDisable()
        {
            try
            {
                MethodInfo disableMethod = DefaultEditor.GetType().GetMethod("OnDisable",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (disableMethod != null)
                    disableMethod.Invoke(DefaultEditor, null);
                DestroyImmediate(DefaultEditor);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public override void OnInspectorGUI()
        {
            try
            {
                DefaultEditor.OnInspectorGUI();
            }
            catch (NullReferenceException e)    //TODO 可能是unity的bug，有时会有null异常
            {
                // ignored
            }
        }

        protected override void OnHeaderGUI()
        {
            DefaultEditor.DrawHeader();
        }

        public override bool HasPreviewGUI()
        {
            return DefaultEditor.HasPreviewGUI();
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            DefaultEditor.OnPreviewGUI(r, background);
        }

        public override void OnPreviewSettings()
        {
            DefaultEditor.OnPreviewSettings();
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            return DefaultEditor.RenderStaticPreview(assetPath, subAssets, width, height);
        }

        public override void ReloadPreviewInstances()
        {
            DefaultEditor.ReloadPreviewInstances();
        }

        public override void DrawPreview(Rect previewArea)
        {
            DefaultEditor.DrawPreview(previewArea);
        }

        // public override VisualElement CreateInspectorGUI()
        // {
        //     return DefaultEditor.CreateInspectorGUI();
        // }

        public override GUIContent GetPreviewTitle()
        {
            return DefaultEditor.GetPreviewTitle();
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            DefaultEditor.OnInteractivePreviewGUI(r, background);
        }
    }

    public abstract class ExtendEditor<T> : ExtendEditor where T : class
    {
        public T Target
        {
            get
            {
                var t = target as T;
                if (t == null)
                    throw new Exception("T is different from the type in the CustomEditor attribute");
                return t;
            }
        }
    }
}
