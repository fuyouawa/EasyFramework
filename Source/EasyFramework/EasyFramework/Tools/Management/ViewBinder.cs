using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace EasyFramework
{
    [Conditional("UNITY_EDITOR")]
    public class ByViewBinderAttribute : PropertyAttribute
    {
        public string OriginName;

        public ByViewBinderAttribute(string originName)
        {
            OriginName = originName;
        }
    }

    [Serializable]
    public class ViewBinderInfo
    {
        public Transform OwnerViewModel;
        public Component BindComponent;

        public SerializedAny EditorData;
    }

    public interface IViewBinder
    {
        ViewBinderInfo Info { get; set; }
    }


    public class ViewBinder : MonoBehaviour, IViewBinder
    {
        [SerializeField] private ViewBinderInfo _viewBinderInfo;

        ViewBinderInfo IViewBinder.Info
        {
            get => _viewBinderInfo;
            set => _viewBinderInfo = value;
        }
    }
}
