using EasyToolKit.ThirdParty.OdinSerializer;
using System;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [CreateAssetMenu(fileName = "TerrainTileRuleSetAsset", menuName = "EasyToolKit/Tilemap/Create TerrainTileRuleSetAsset")]
    public class TerrainTileRuleSetAsset : ScriptableObject, ISerializationCallbackReceiver
    {
        private Guid _guid;
        [SerializeField, HideInInspector] private byte[] _serializedGuid;

        [SerializeField] private TerrainTileFillRule _fillRule;
        [SerializeField] private TerrainTileEdgeRule _edgeRule;
        [SerializeField] private TerrainTileExteriorCornerRule _exteriorCornerRule;
        [SerializeField] private TerrainTileInteriorCornerRule _interiorCornerRule;

        public Guid Guid => _guid;
        public TerrainTileFillRule FillRule => _fillRule;
        public TerrainTileEdgeRule EdgeRule => _edgeRule;
        public TerrainTileExteriorCornerRule ExteriorCornerRule => _exteriorCornerRule;
        public TerrainTileInteriorCornerRule InteriorCornerRule => _interiorCornerRule;

        public GameObject GetTileInstanceByRuleType(TerrainRuleType ruleType)
        {
            switch (ruleType)
            {
                case TerrainRuleType.Fill:
                    return FillRule.GetTileInstanceByRuleType(ruleType);
                case TerrainRuleType.TopEdge:
                case TerrainRuleType.LeftEdge:
                case TerrainRuleType.BottomEdge:
                case TerrainRuleType.RightEdge:
                    return EdgeRule.GetTileInstanceByRuleType(ruleType);
                case TerrainRuleType.TopLeftExteriorCorner:
                case TerrainRuleType.TopRightExteriorCorner:
                case TerrainRuleType.BottomRightExteriorCorner:
                case TerrainRuleType.BottomLeftExteriorCorner:
                    return ExteriorCornerRule.GetTileInstanceByRuleType(ruleType);
                case TerrainRuleType.TopLeftInteriorCorner:
                case TerrainRuleType.TopRightInteriorCorner:
                case TerrainRuleType.BottomRightInteriorCorner:
                case TerrainRuleType.BottomLeftInteriorCorner:
                    return InteriorCornerRule.GetTileInstanceByRuleType(ruleType);
                default:
                    throw new ArgumentOutOfRangeException(nameof(ruleType), ruleType, null);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (_guid == Guid.Empty)
            {
                _guid = Guid.NewGuid();
            }

            _serializedGuid = SerializationUtility.SerializeValue(_guid, DataFormat.Binary);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_serializedGuid != null && _serializedGuid.Length > 0)
            {
                _guid = SerializationUtility.DeserializeValue<Guid>(_serializedGuid, DataFormat.Binary);
            }
            else
            {
                _guid = Guid.NewGuid();
            }
        }
    }
}
