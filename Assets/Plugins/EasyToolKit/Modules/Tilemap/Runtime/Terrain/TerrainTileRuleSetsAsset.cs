using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [CreateAssetMenu(fileName = "TerrainTileRuleSetsAsset", menuName = "EasyToolKit/Tilemap/Create TerrainTileRuleSetsAsset")]
    public class TerrainTileRuleSetsAsset : ScriptableObject
    {
        public TerrainTileFillRuleSet FillRuleSet;
        public TerrainTileEdgeRuleSet EdgeRuleSet;
        public TerrainTileExteriorCornerRuleSet ExteriorCornerRuleSet;
        public TerrainTileInteriorCornerRuleSet InteriorCornerRuleSet;
    }
}
