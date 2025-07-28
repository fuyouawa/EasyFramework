using EasyToolKit.Inspector;
using System;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [Serializable]
    public class TerrainTileDefinition : ISerializationCallbackReceiver, IEquatable<TerrainTileDefinition>
    {
        private Guid _guid;

        [LabelText("名称")]
        [SerializeField] private string _name;

        [LabelText("调试块颜色")]
        [SerializeField] private Color _debugCubeColor = Color.green;

        [LabelText("绘制调试块")]
        [SerializeField] private bool _drawDebugCube = false;

        [InlineEditor(Style = InlineEditorStyle.Foldout)]
        [LabelText("规则集")]
        [SerializeField] private TerrainTileRuleSetsAsset _ruleSetsAsset;

        [SerializeField, HideInInspector] private byte[] _serializedGuid;
        [SerializeField, HideInInspector] private byte[] _serializedMapData;

        public Guid Guid => _guid;
        public string Name => _name;
        public Color DebugCubeColor => _debugCubeColor;
        public bool DrawDebugCube => _drawDebugCube;
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

        public static bool operator==(TerrainTileDefinition left, TerrainTileDefinition right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);

            return left.Equals(right);
        }

        public static bool operator !=(TerrainTileDefinition left, TerrainTileDefinition right)
        {
            return !(left == right);
        }

        public bool Equals(TerrainTileDefinition obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return _guid == obj._guid;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TerrainTileDefinition)obj);
        }

        public override int GetHashCode()
        {
            return _guid.GetHashCode();
        }
    }
}
