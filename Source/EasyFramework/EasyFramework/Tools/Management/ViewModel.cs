using System;
using UnityEngine;

namespace EasyFramework
{
    [Serializable]
    public class ViewModelInfo
    {
        public SerializedAny EditorData;
    }

    public interface IViewModel
    {
        ViewModelInfo Info { get; set; }
    }

    public sealed class ViewModel : MonoBehaviour, IViewModel
    {
        [SerializeField] private ViewModelInfo _viewModelInfo;

        ViewModelInfo IViewModel.Info
        {
            get => _viewModelInfo;
            set => _viewModelInfo = value;
        }
    }
}
