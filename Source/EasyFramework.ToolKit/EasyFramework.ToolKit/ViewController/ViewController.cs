using System;
using System.Collections.Generic;
using EasyFramework.Serialization;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public enum ViewControllerBindersGroupType
    {
        None,
        Title,
        Foldout
    }

    [Serializable]
    public class ViewControllerEditorConfig : ISerializationCallbackReceiver
    {
        public bool IsInitialized;
        public string GenerateDir;
        public string Namespace;
        public bool AutoScriptName = true;
        public string ScriptName;
        public List<OtherViewBinders> OtherBindersList;
        public Type BaseClass;

        public ViewControllerBindersGroupType BindersGroupType;
        public string BindersGroupName;

        [SerializeField, HideInInspector]
        private EasySerializationData _serializationData;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            EasySerialize.To(this, ref _serializationData);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            var obj = EasySerialize.From<ViewControllerEditorConfig>(ref _serializationData);
            if (obj != null)
                EasySerializationUtility.AutoCopy(obj, this);
        }
    }

    [Serializable]
    public class ViewControllerConfig
    {
        public ViewControllerEditorConfig EditorConfig;
    }

    public interface IViewController
    {
        ViewControllerConfig Config { get; set; }
    }

    public sealed class ViewController : MonoBehaviour, IViewController
    {
        [SerializeField] private ViewControllerConfig _viewControllerConfig;

        ViewControllerConfig IViewController.Config
        {
            get => _viewControllerConfig;
            set => _viewControllerConfig = value;
        }
    }
}
