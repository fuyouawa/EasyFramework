using System;
using System.Collections.Generic;
using EasyFramework.Core;
using EasyFramework.Serialization;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    [Serializable]
    public class ViewControllerConfig : ISerializationCallbackReceiver
    {
        public enum GroupType
        {
            None,
            Title,
            Foldout
        }

        [SerializeField] private string _generateDirectory;
        [SerializeField] private string _namespace;
        [SerializeField] private bool _autoScriptName = true;
        [SerializeField] private string _scriptName;
        [SerializeField] private Type _baseClass;

        [SerializeField] private GroupType _bindersGroupType;
        [SerializeField] private string _bindersGroupName;

        [SerializeField] private bool _isInitialized;

        [SerializeField, HideInInspector]
        private EasySerializationData _serializedData;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _serializedData.Format = EasyDataFormat.Json;
            EasySerialize.To(this, ref _serializedData);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_serializedData.GetData().IsNullOrEmpty())
            {
                return;
            }

            var obj = EasySerialize.From<ViewControllerConfig>(ref _serializedData);
            if (obj != null)
            {
                _isInitialized = obj._isInitialized;
                _generateDirectory = obj._generateDirectory;
                _namespace = obj._namespace;
                _autoScriptName = obj._autoScriptName;
                _scriptName = obj._scriptName;
                _baseClass = obj._baseClass;
                _bindersGroupType = obj._bindersGroupType;
                _bindersGroupName = obj._bindersGroupName;
            }
        }

        public string Namespace => _namespace;
        public string GenerateDirectory => _generateDirectory;
        public Type BaseClass => _baseClass;
        public GroupType BindersGroupType => _bindersGroupType;
        public string BindersGroupName => _bindersGroupName;


        public string GetScriptName(Component component)
        {
            var name = _scriptName;

            if (_autoScriptName)
            {
                name = component.gameObject.name;
            }

            return name;
        }
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
