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

        private PropertyTree _tree;
        private object _audioFilterGUIInstance;

        public PropertyTree Tree
        {
            get
            {
                if (_tree == null)
                {
                    try
                    {
                        _tree = PropertyTree.Create(serializedObject);
                    }
                    catch (ArgumentException)
                    {
                    }
                }

                return _tree;
            }
        }

        public bool IsInlineEditor { get; set; }

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
            if (Tree == null)
            {
                base.OnInspectorGUI();
                return;
            }


            if (Event.current.type == EventType.Layout)
            {
                Tree.DrawMonoScriptObjectField = Tree.SerializedObject != null &&
                                                 Tree.TargetType != null &&
                                                 InspectorConfigAsset.Instance.DrawMonoScriptInEditor;
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
            Tree.Draw();
        }
    }
}
