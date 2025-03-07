using System;
using System.Diagnostics;
using EasyFramework.Serialization;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    [Conditional("UNITY_EDITOR")]
    public class AutoBindingAttribute : PropertyAttribute
    {
        public AutoBindingAttribute()
        {
        }
    }

    public enum ViewBindAccess
    {
        Public,
        Protected,
        Private
    }

    [Serializable]
    public class ViewBinderEditorConfig : ISerializationCallbackReceiver
    {
        public bool IsInitialized;
        public bool BindGameObject;
        public Type BindComponentType;
        public Type SpecificBindType;
        public ViewBindAccess BindAccess;
        public bool AutoBindName = true;
        public string BindName;
        public bool ProcessBindName = true;
        public bool UseDocumentComment = true;
        public bool AutoAddParaToComment = true;
        [TextArea(4, 10)]
        public string Comment;

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
    public class ViewBinderConfig
    {
        public Component OwnerController;
        public ViewBinderEditorConfig EditorConfig;
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
