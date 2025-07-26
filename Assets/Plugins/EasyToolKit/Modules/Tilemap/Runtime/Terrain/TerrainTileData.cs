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
        private List<List<byte>> _mapData;

        [LabelText("名称")]
        public string Name;

        [LabelText("瓦片颜色")]
        public Color Color = Color.green;

        [InlineEditor(Style = InlineEditorStyle.Foldout)]
        [LabelText("地形瓦片规则集")]
        public TerrainTileRuleSetsAsset RuleSetsAsset;

        public Guid Guid => _guid;

        [SerializeField, HideInInspector]
        private byte[] _serializedGuid;

        [SerializeField, HideInInspector]
        private byte[] _serializedMapData;

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
                _mapData = SerializationUtility.DeserializeValue<List<List<byte>>>(_serializedMapData,
                    DataFormat.Binary);
            }
            else
            {
                _mapData = new List<List<byte>>();
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
