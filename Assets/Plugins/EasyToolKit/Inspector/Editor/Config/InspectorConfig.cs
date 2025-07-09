using System;
using System.Reflection;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Core.Editor.Internal;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;

namespace EasyToolKit.Inspector.Editor
{
    [EditorConfigsPath("Inspector")]
    public class InspectorConfig : ScriptableObjectSingleton<InspectorConfig>, ISerializationCallbackReceiver
    {
        private static bool hasUpdatedEditorsOnce = false;
        private static bool IsHeadlessMode => SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;

        [SerializeField, HideInInspector] private InspectorDefaultEditors defaultEditorBehaviour = InspectorDefaultEditors.UserTypes | InspectorDefaultEditors.PluginTypes | InspectorDefaultEditors.OtherTypes;

        [SerializeField] private bool _enableEasyInspector;

        [SerializeField] private InspectorTypeDrawingConfig _drawingConfig = new InspectorTypeDrawingConfig();

        
        /// <summary>
        /// InspectorDefaultEditors is a bitmask used to tell which types should have an Odin Editor generated.
        /// </summary>
        public InspectorDefaultEditors DefaultEditorBehaviour
        {
            get { return this.defaultEditorBehaviour; }
            set { this.defaultEditorBehaviour = value; }
        }

        public void UpdateOdinEditors()
        {
            if (InspectorConfig.IsHeadlessMode || InternalEditorUtility.inBatchMode)
                return;

            CustomEditorUtility.ResetCustomEditors();

            if (this._enableEasyInspector)
            {
                foreach (TypeDrawerPair typeDrawerPair in _drawingConfig.GetEditors())
                {
                    var drawnType = AssemblyUtility.GetTypeByFullName(typeDrawerPair.DrawnTypeName);
                    var editorType = AssemblyUtility.GetTypeByFullName(typeDrawerPair.EditorTypeName);

                    if (drawnType == null || editorType == null) continue;

                    CustomEditorUtility.SetCustomEditor(drawnType, editorType, isFallbackEditor: false, isEditorForChildClasses: false);
                }
            }

            EditorApplication.delayCall += (EditorApplication.CallbackFunction)(() =>
            {
                Type inspectorWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
                Type activeEditorTrackerType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ActiveEditorTracker");

                if (inspectorWindowType != null && activeEditorTrackerType != null)
                {
                    var createTrackerMethod = inspectorWindowType.GetMethod("CreateTracker", BindingFlagsHelper.AllInstance());
                    var trackerField = inspectorWindowType.GetField("m_Tracker", BindingFlagsHelper.AllInstance());
                    var forceRebuild = activeEditorTrackerType.GetMethod("ForceRebuild", BindingFlagsHelper.AllInstance());

                    if (createTrackerMethod != null && trackerField != null && forceRebuild != null)
                    {
                        var windows = Resources.FindObjectsOfTypeAll(inspectorWindowType);

                        foreach (var window in windows)
                        {
                            createTrackerMethod.Invoke(window, null);
                            object tracker = trackerField.GetValue(window);
                            forceRebuild.Invoke(tracker, null);
                        }
                    }
                }
            });
        }

        public void UpdateAndRefreshInspector()
        {
            this.UpdateOdinEditors();
            UnityEngine.Object[] objects = Selection.objects;
            Selection.objects = null;
            EditorApplication.delayCall +=(() => Selection.objects = objects);
        }

        internal void EnsureEditorsHaveBeenUpdated()
        {
            if (hasUpdatedEditorsOnce == false)
            {
                this.UpdateOdinEditors();
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _drawingConfig.UpdateCaches();
        }
    }
}
