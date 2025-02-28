using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    [Serializable]
    public class ViewControllerEditorConfig : Internal.IValueDrawerHelper
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
