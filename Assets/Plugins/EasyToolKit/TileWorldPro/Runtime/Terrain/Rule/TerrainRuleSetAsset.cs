using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [CreateAssetMenu(fileName = "TerrainRuleSet", menuName = "EasyToolKit/TileWorldPro/TerrainRuleSet")]
    public class TerrainRuleSetAsset : ScriptableObject
    {
        [SerializeField] private TerrainFillRule _fillRule;
        [SerializeField] private TerrainEdgeRule _edgeRule;
        [SerializeField] private TerrainExteriorCornerRule _exteriorCornerRule;
        [SerializeField] private TerrainInteriorCornerRule _interiorCornerRule;

        public TerrainFillRule FillRule => _fillRule;
        public TerrainEdgeRule EdgeRule => _edgeRule;
        public TerrainExteriorCornerRule ExteriorCornerRule => _exteriorCornerRule;
        public TerrainInteriorCornerRule InteriorCornerRule => _interiorCornerRule;

        public GameObject GetTileInstanceByRuleType(TerrainTileRuleType ruleType)
        {
            switch (ruleType)
            {
                case TerrainTileRuleType.Fill:
                    return _fillRule.GetTileInstanceByRuleType(ruleType);
                case TerrainTileRuleType.TopEdge:
                case TerrainTileRuleType.LeftEdge:
                case TerrainTileRuleType.BottomEdge:
                case TerrainTileRuleType.RightEdge:
                    return _edgeRule.GetTileInstanceByRuleType(ruleType);
                case TerrainTileRuleType.TopLeftExteriorCorner:
                case TerrainTileRuleType.TopRightExteriorCorner:
                case TerrainTileRuleType.BottomRightExteriorCorner:
                case TerrainTileRuleType.BottomLeftExteriorCorner:
                    return _exteriorCornerRule.GetTileInstanceByRuleType(ruleType);
                case TerrainTileRuleType.TopLeftInteriorCorner:
                case TerrainTileRuleType.TopRightInteriorCorner:
                case TerrainTileRuleType.BottomRightInteriorCorner:
                case TerrainTileRuleType.BottomLeftInteriorCorner:
                    return _interiorCornerRule.GetTileInstanceByRuleType(ruleType);
                default:
                    throw new ArgumentOutOfRangeException(nameof(ruleType), ruleType, null);
            }
        }
    }
}
