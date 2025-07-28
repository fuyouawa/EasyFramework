using System;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;
using UnityEngine.Serialization;

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
