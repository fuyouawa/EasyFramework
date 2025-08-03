using System;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [CreateAssetMenu(fileName = "TerrainRuleSetAsset", menuName = "EasyToolKit/Tilemap/Create TerrainRuleSetAsset")]
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
                    return FillRule.GetTileInstanceByRuleType(ruleType);
                case TerrainTileRuleType.TopEdge:
                case TerrainTileRuleType.LeftEdge:
                case TerrainTileRuleType.BottomEdge:
                case TerrainTileRuleType.RightEdge:
                    return EdgeRule.GetTileInstanceByRuleType(ruleType);
                case TerrainTileRuleType.TopLeftExteriorCorner:
                case TerrainTileRuleType.TopRightExteriorCorner:
                case TerrainTileRuleType.BottomRightExteriorCorner:
                case TerrainTileRuleType.BottomLeftExteriorCorner:
                    return ExteriorCornerRule.GetTileInstanceByRuleType(ruleType);
                case TerrainTileRuleType.TopLeftInteriorCorner:
                case TerrainTileRuleType.TopRightInteriorCorner:
                case TerrainTileRuleType.BottomRightInteriorCorner:
                case TerrainTileRuleType.BottomLeftInteriorCorner:
                    return InteriorCornerRule.GetTileInstanceByRuleType(ruleType);
                default:
                    throw new ArgumentOutOfRangeException(nameof(ruleType), ruleType, null);
            }
        }
    }
}
