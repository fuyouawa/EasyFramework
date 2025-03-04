using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EasyFramework.Serialization;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    [Conditional("UNITY_EDITOR")]
    public class FromViewBinderAttribute : PropertyAttribute
    {
        public FromViewBinderAttribute()
        {
        }
    }

    public enum ViewBindAccess
    {
        Public,
        Protect,
        Private
    }

    [Serializable]
    public class ViewBinderEditorConfig : Internal.IValueDrawerHelper, ISerializationCallbackReceiver
    {
        public bool IsInitialized;

        public bool BindGameObject;
        
        [ShowInInspector]
        [ShowIf(nameof(ShowBindComponent))]
        [ValueDropdown(nameof(GetBindComponentTypeDropdown))]
        public Type BindComponentType;
        
        [ShowInInspector]
        [ShowIf(nameof(ShowSpecificBindType))]
        [ValueDropdown(nameof(GetSpecificBindTypeDropdown))]
        public Type SpecificBindType;

        public ViewBindAccess BindAccess;
        public bool AutoBindName = true;
        public string BindName;

        public bool ProcessBindName = true;

        public bool UseDocumentComment = true;

        [ShowIf(nameof(ShowAutoAddParaToComment))]
        public bool AutoAddParaToComment = true;

        [TextArea(3, 5)]
        public string Comment;

        private bool ShowBindComponent => !BindGameObject;
        private bool ShowSpecificBindType => !BindGameObject && BindComponentType != null;
        private bool ShowAutoAddParaToComment => UseDocumentComment;
        
        Component Internal.IValueDrawerHelper.TargetComponent
        {
            get => _targetComponent;
            set => _targetComponent = value;
        }

        private Component _targetComponent;

        private IEnumerable GetBindComponentTypeDropdown()
        {
            if (_targetComponent == null)
                return Enumerable.Empty<Component>();

            return ViewBinderUtility.GetBindableComponentTypes((IViewBinder)_targetComponent);
        }

        private IEnumerable GetSpecificBindTypeDropdown()
        {
            if (BindComponentType == null)
                return Enumerable.Empty<Type>();

            return ViewBinderUtility.GetSpecficableBindTypes(BindComponentType);
        }

        [SerializeField, HideInInspector]
        private EasySerializationData _serializationData;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            EasySerialize.To(this, ref _serializationData);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            var obj = EasySerialize.From<ViewBinderEditorConfig>(ref _serializationData);
            if (obj != null)
                EasySerializationUtility.AutoCopy(obj, this);
        }
    }

    [Serializable]
    public class ViewBinderConfig : Internal.IValueDrawerHelper
    {
        public IViewController OwnerController => _ownerController as IViewController;

        [SerializeField]
        [ValueDropdown(nameof(GetOwnerControllerDropdown))]
        private Component _ownerController;

        public ViewBinderEditorConfig EditorConfig;

        Component Internal.IValueDrawerHelper.TargetComponent
        {
            get => _targetComponent;
            set => _targetComponent = value;
        }

        private Component _targetComponent;

        IEnumerable GetOwnerControllerDropdown()
        {
            if (_targetComponent == null)
                return Enumerable.Empty<Component>();

            return _targetComponent.gameObject
                .GetComponentsInParent(typeof(IViewController), true);
        }
    }

    public interface IViewBinder
    {
        ViewBinderConfig Config { get; set; }
    }


    public class ViewBinder : MonoBehaviour, IViewBinder
    {
        [SerializeField] private ViewBinderConfig _viewBinderConfig;

        ViewBinderConfig IViewBinder.Config
        {
            get => _viewBinderConfig;
            set => _viewBinderConfig = value;
        }
    }
}
