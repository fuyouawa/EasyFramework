using System;
using System.Reflection;
using EasyToolKit.ThirdParty.OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;
using UnityVersion = EasyToolKit.Core.UnityVersion;

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

        private PropertyTree _propertyTree;
        private object _audioFilterGUIInstance;

        public PropertyTree PropertyTree
        {
            get
            {
                if (_propertyTree == null)
                {
                    try
                    {
                        _propertyTree = PropertyTree.Create(serializedObject);
                    }
                    catch (ArgumentException)
                    {
                    }
                }

                return _propertyTree;
            }
        }

        public override void OnInspectorGUI()
        {
            DrawInspector();
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
                    s_drawAudioFilterGUI = EmitUtilities.CreateWeakInstanceMethodCaller<MonoBehaviour>(
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

        private void DrawInspector()
        {
            if (PropertyTree == null)
            {
                base.OnInspectorGUI();
                return;
            }

            
            if (Event.current.type == EventType.Layout)
            {
                PropertyTree.DrawMonoScriptObjectField = PropertyTree.SerializedObject != null &&
                                                         PropertyTree.TargetType != null;
            }

            using (new LocalizationGroup(target))
            {
                DrawTree();

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
        }

        protected virtual void DrawTree()
        {
            PropertyTree.Draw();
        }
    }
}
