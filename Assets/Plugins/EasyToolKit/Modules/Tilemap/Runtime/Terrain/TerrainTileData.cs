using EasyToolKit.Inspector;
using System.Collections.Generic;
using System;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [Serializable]
    public class TerrainTileData : ISerializationCallbackReceiver, IEquatable<TerrainTileData>
    {
        private Guid _guid;
        private Dictionary<Vector3Int, byte> _mapData;

        [LabelText("名称")]
        [SerializeField] private string _name;

        [LabelText("瓦片颜色")]
        [SerializeField] private Color _color = Color.green;

        [InlineEditor(Style = InlineEditorStyle.Foldout)]
        [LabelText("地形瓦片规则集")]
        [SerializeField] private TerrainTileRuleSetsAsset _ruleSetsAsset;

        [SerializeField, HideInInspector] private byte[] _serializedGuid;
        [SerializeField, HideInInspector] private byte[] _serializedMapData;

        public Guid Guid => _guid;
        public string Name => _name;
        public Color Color => _color;
        public TerrainTileRuleSetsAsset RuleSetsAsset => _ruleSetsAsset;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (_guid == Guid.Empty)
            {
                _guid = Guid.NewGuid();
            }

            _serializedGuid = SerializationUtility.SerializeValue(_guid, DataFormat.Binary);
            _serializedMapData = SerializationUtility.SerializeValue(_mapData, DataFormat.Binary);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_serializedMapData != null && _serializedMapData.Length > 0)
            {
                _mapData = SerializationUtility.DeserializeValue<Dictionary<Vector3Int, byte>>(_serializedMapData,
                    DataFormat.Binary);
            }
            else
            {
                _mapData = new Dictionary<Vector3Int, byte>();
            }

            if (_serializedGuid != null && _serializedGuid.Length > 0)
            {
                _guid = SerializationUtility.DeserializeValue<Guid>(_serializedGuid, DataFormat.Binary);
            }
            else
            {
                _guid = Guid.NewGuid();
            }
        }

        public bool Equals(TerrainTileData other)
        {
            if (other == null) return false;
            return _guid == other._guid;
        }
    }
}
