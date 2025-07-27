using EasyToolKit.Inspector;
using System;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [Serializable]
    public class TerrainTile : ISerializationCallbackReceiver, IEquatable<TerrainTile>
    {
        private Guid _guid;

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

        public bool Equals(TerrainTile other)
        {
            if (other == null) return false;
            return _guid == other._guid;
        }
    }
}
