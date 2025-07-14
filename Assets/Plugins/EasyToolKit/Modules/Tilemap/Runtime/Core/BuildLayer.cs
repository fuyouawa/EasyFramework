using System;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [Serializable]
    public class BuildLayer
    {
        [SerializeField, HideInInspector] private Guid _generationLayerGuid;
        [SerializeField] private string _layerName;
        [SerializeField] private bool _enabled;
    }
}
