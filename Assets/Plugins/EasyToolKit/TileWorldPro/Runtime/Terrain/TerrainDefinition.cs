using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;
using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [Serializable]
    public class TerrainDefinition : ISerializationCallbackReceiver, IEquatable<TerrainDefinition>
    {
        private Guid _guid;

        [LabelText("名称")]
        [SerializeField] private string _name;

        [InlineEditor(Style = InlineEditorStyle.Foldout)]
        [LabelText("规则集")]
        [SerializeField] private TerrainRuleSetAsset _ruleSetAsset;

        [FoldoutGroup("调试")]
        [LabelText("调试块颜色")]
        [SerializeField] private Color _debugCubeColor = new Color(0, 1, 0, 0.5f);

        [LabelText("绘制调试块")]
        [SerializeField] private bool _drawDebugCube = true;

        [LabelText("为调试块启用深度测试")]
        [SerializeField] private bool _enableZTestForDebugCube = true;

        [SerializeField, HideInInspector] private byte[] _serializedGuid;
        [SerializeField, HideInInspector] private byte[] _serializedMapData;

        public Guid Guid => _guid;
        public string Name => _name;
        public Color DebugCubeColor => _debugCubeColor;
        public bool DrawDebugCube => _drawDebugCube;
        public bool EnableZTestForDebugCube => _enableZTestForDebugCube;
        public TerrainRuleSetAsset RuleSetAsset => _ruleSetAsset;

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

        public static bool operator ==(TerrainDefinition left, TerrainDefinition right)
        {
            if (left is null)
                return right is null;

            return left.Equals(right);
        }

        public static bool operator !=(TerrainDefinition left, TerrainDefinition right)
        {
            return !(left == right);
        }

        public bool Equals(TerrainDefinition obj)
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
            return Equals((TerrainDefinition)obj);
        }

        public override int GetHashCode()
        {
            return _guid.GetHashCode();
        }
    }
}
