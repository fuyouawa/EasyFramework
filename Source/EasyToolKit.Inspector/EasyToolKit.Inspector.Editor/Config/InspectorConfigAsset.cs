using System;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Core.Editor.Internal;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace EasyToolKit.Inspector.Editor
{
    [EditorConfigsPath]
    public class InspectorConfigAsset : ScriptableObjectSingleton<InspectorConfigAsset>, ISerializationCallbackReceiver
    {
        public bool DrawMonoScriptInEditor = true;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnityEditorEventUtility.DelayAction(UpdateEditors);
        }

        public void UpdateEditors()
        {
            var types = TypeCache.GetTypesWithAttribute<EasyInspectorAttribute>();
            var drawnTypes = types.Where(type => type.IsSubclassOf(typeof(Component)) ||
                                                 type.IsSubclassOf(typeof(ScriptableObject)));
            foreach (var drawnType in drawnTypes)
            {
                CustomEditorUtility.SetCustomEditor(drawnType, typeof(EasyEditor), false, false);
            }

            EditorApplication.delayCall += () =>
            {
                Type inspectorWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
                Type activeEditorTrackerType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ActiveEditorTracker");

                if (inspectorWindowType != null && activeEditorTrackerType != null)
                {
                    var createTrackerMethod =
                        inspectorWindowType.GetMethod("CreateTracker", BindingFlagsHelper.AllInstance());
                    var trackerField = inspectorWindowType.GetField("m_Tracker", BindingFlagsHelper.AllInstance());
                    var forceRebuild =
                        activeEditorTrackerType.GetMethod("ForceRebuild", BindingFlagsHelper.AllInstance());

                    if (createTrackerMethod != null && trackerField != null && forceRebuild != null)
                    {
                        // 获取所有检查器窗口并强制重建
                        var windows = Resources.FindObjectsOfTypeAll(inspectorWindowType);

                        foreach (var window in windows)
                        {
                            createTrackerMethod.Invoke(window, null);
                            object tracker = trackerField.GetValue(window);
                            forceRebuild.Invoke(tracker, null);
                        }
                    }
                }
            };
        }
    }
}
