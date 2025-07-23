using System;
using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [EasyInspector]
    [CreateAssetMenu(fileName = "TerrainTileSet", menuName = "EasyToolKit/Tilemap/Create TerrainTileSet")]
    public class TerrainTileSetAsset : ScriptableObject
    {
        public TerrainTileFillRuleSet FillRuleSet;
        public TerrainTileEdgeRuleSet EdgeRuleSet;
        public TerrainTileExteriorCornerRuleSet ExteriorCornerRuleSet;
        public TerrainTileInteriorCornerRuleSet InteriorCornerRuleSet;
    }
}
