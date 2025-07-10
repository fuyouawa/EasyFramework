using System;
using System.Reflection;
using EasyToolKit.Core;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [InitializeOnLoad]
    [CanEditMultipleObjects]
    public class EasyEditor : UnityEditor.Editor
    {
        private static Func<MonoBehaviour, int> s_getCustomFilterChannelCount;
        private static Func<MonoBehaviour, bool> s_haveAudioCallback;
        private static Action<object, MonoBehaviour> s_drawAudioFilterGUI;

        private static bool s_hasReflectedAudioFilter;
        private static bool s_initialized = false;
        private static Type s_audioFilterGUIType;

        private object _audioFilterGUIInstance;

        public override void OnInspectorGUI()
        {
            DrawEasyInspector();
        }

        protected virtual void OnEnable()
        {
            EnsureInitialized();
        }

        protected virtual void OnDisable()
        {
        }


        private static void EnsureInitialized()
        {
            if (!s_initialized)
            {
                s_initialized = true;

                try
                {
                    string haveAudioCallbackName =
                        UnityVersion.IsVersionOrGreater(5, 6) ? "HasAudioCallback" : "HaveAudioCallback";

                    s_haveAudioCallback = (Func<MonoBehaviour, bool>)Delegate.CreateDelegate(
                        typeof(Func<MonoBehaviour, bool>),
                        typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.AudioUtil")
                            .GetMethod(haveAudioCallbackName, BindingFlags.Public | BindingFlags.Static));

                    s_getCustomFilterChannelCount = (Func<MonoBehaviour, int>)Delegate.CreateDelegate(
                        typeof(Func<MonoBehaviour, int>),
                        typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.AudioUtil")
                            .GetMethod("GetCustomFilterChannelCount", BindingFlags.Public | BindingFlags.Static));

                    s_audioFilterGUIType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.AudioFilterGUI");
                    s_drawAudioFilterGUI = EmitUtility.CreateWeakInstanceMethodCaller<MonoBehaviour>(
                        s_audioFilterGUIType.GetMethod("DrawAudioFilterGUI",
                            BindingFlags.Public | BindingFlags.Instance));
                    s_hasReflectedAudioFilter = true;
                }
                catch (Exception)
                {
                    Debug.LogWarning(
                        "The internal Unity class AudioFilterGUI has been changed; cannot properly mock a generic Unity inspector. This probably won't be very noticeable.");
                }
            }
        }

        private bool DrawEasyInspector()
        {
            bool res;
            using (new LocalizationGroup(target))
            {
                res = DrawEasyInspectorImpl();

                if (s_hasReflectedAudioFilter && this.target is MonoBehaviour)
                {
                    if (s_haveAudioCallback(this.target as MonoBehaviour) &&
                        s_getCustomFilterChannelCount(this.target as MonoBehaviour) > 0)
                    {
                        if (this._audioFilterGUIInstance == null)
                        {
                            this._audioFilterGUIInstance = Activator.CreateInstance(s_audioFilterGUIType);
                        }

                        s_drawAudioFilterGUI(this._audioFilterGUIInstance, this.target as MonoBehaviour);
                    }
                }
            }

            return res;
        }

        private bool DrawEasyInspectorImpl()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();

            // Loop through properties and create one field (including children) for each top level property.
            SerializedProperty property = serializedObject.GetIterator();
            bool expanded = true;
            while (property.NextVisible(expanded))
            {
                DrawProperty(property);
                expanded = false;
            }

            serializedObject.ApplyModifiedProperties();

            return EditorGUI.EndChangeCheck();
        }

        private void DrawProperty(SerializedProperty property)
        {
            using (new EditorGUI.DisabledScope("m_Script" == property.propertyPath))
            {
                EditorGUILayout.PropertyField(property, true);
            }

            if (property.propertyPath == "m_Script")
            {
                MonoScript monoScript = property.objectReferenceValue as MonoScript;

                bool invalid = !(monoScript != null);

                if (invalid)
                {
                    EditorGUILayout.HelpBox(
                        "The associated script can not be loaded.\nPlease fix any compile errors\nand assign a valid script.",
                        MessageType.Warning);
                }

                if (serializedObject.ApplyModifiedProperties())
                {
                    ActiveEditorTracker.sharedTracker.ForceRebuild();
                }
            }
        }
    }
}
