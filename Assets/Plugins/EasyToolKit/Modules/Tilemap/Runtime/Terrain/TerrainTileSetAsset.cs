using System;
using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [EasyInspector]
    [CreateAssetMenu(fileName = "TerrainTileSet", menuName = "EasyToolKit/Tilemap/Create TerrainTileSet")]
    public class TerrainTileSetAsset : ScriptableObject
    {
        [AwesomeBoxGroup("地形瓦片规则集")]
        public TerrainTileFillRuleSet FillRuleSet;
        public TerrainTileEdgeRuleSet EdgeRuleSet;
        public TerrainTileExteriorCornerRuleSet ExteriorCornerRuleSet;
        public TerrainTileInteriorCornerRuleSet InteriorCornerRuleSet;
    }
}
