using System;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [Serializable]
    public class BlueprintLayer
    {
        [SerializeField] private string _layerName;
        [SerializeField] private bool _enabled;
        [SerializeField] private Color _gridColor;
    }
}
