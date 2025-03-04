using System;
using System.Collections.Generic;
using EasyFramework.Serialization;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    [Serializable]
    public class ViewControllerEditorConfig : Internal.IValueDrawerHelper, ISerializationCallbackReceiver
    {
        public bool IsInitialized;

        [FolderPath(ParentFolder = "Assets")]
        public string GenerateDir;
        public string Namespace;

        public bool AutoScriptName = true;
        public string ScriptName;
        
        Component Internal.IValueDrawerHelper.TargetComponent { get => _targetComponent; set => _targetComponent = value; }
        private Component _targetComponent;

        public List<OtherViewBinders> OtherBindersList;

        [ShowInInspector]
        [TypeDrawerSettings(BaseType = typeof(Component), Filter = TYPE_FILTER)]
        public Type BaseClass;

        private const TypeInclusionFilter TYPE_FILTER =
            TypeInclusionFilter.IncludeConcreteTypes | TypeInclusionFilter.IncludeAbstracts;

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

    public sealed class ViewController : SerializedBehaviour, IViewController
    {
        [SerializeField] private ViewControllerConfig _viewControllerConfig;

        ViewControllerConfig IViewController.Config
        {
            get => _viewControllerConfig;
            set => _viewControllerConfig = value;
        }
    }
}
