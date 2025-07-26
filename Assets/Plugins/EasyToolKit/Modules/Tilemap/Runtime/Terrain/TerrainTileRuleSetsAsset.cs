using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [CreateAssetMenu(fileName = "TerrainTileRuleSetsAsset", menuName = "EasyToolKit/Tilemap/Create TerrainTileRuleSetsAsset")]
    public class TerrainTileRuleSetsAsset : ScriptableObject
    {
        [SerializeField] private TerrainTileFillRuleSet _fillRuleSet;
        [SerializeField] private TerrainTileEdgeRuleSet _edgeRuleSet;
        [SerializeField] private TerrainTileExteriorCornerRuleSet _exteriorCornerRuleSet;
        [SerializeField] private TerrainTileInteriorCornerRuleSet _interiorCornerRuleSet;

        public TerrainTileFillRuleSet FillRuleSet => _fillRuleSet;
        public TerrainTileEdgeRuleSet EdgeRuleSet => _edgeRuleSet;
        public TerrainTileExteriorCornerRuleSet ExteriorCornerRuleSet => _exteriorCornerRuleSet;
        public TerrainTileInteriorCornerRuleSet InteriorCornerRuleSet => _interiorCornerRuleSet;
    }
}
