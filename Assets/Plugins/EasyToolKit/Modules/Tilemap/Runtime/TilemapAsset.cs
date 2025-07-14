using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [CreateAssetMenu(menuName = "EasyTileCreator/Create TilemapAsset", fileName = "TilemapAsset")]
    public class TilemapAsset : ScriptableObject
    {
        [SerializeField] private List<BlueprintLayer> _blueprintLayers = new List<BlueprintLayer>();
        [SerializeField] private List<BuildLayer> _buildLayers = new List<BuildLayer>();
    }
}
