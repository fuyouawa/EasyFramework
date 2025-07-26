using System;
using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TerrainTileRuleTypeAttribute : Attribute
    {
        public TerrainType TerrainType;

        public TerrainTileRuleTypeAttribute(TerrainType terrainType)
        {
            TerrainType = terrainType;
        }
    }

    [Serializable]
    [HideLabel]
    public class TerrainTileRule
    {
        [LabelText("预制体")]
        [SerializeField] private GameObject _tilePrefab;

        [LabelText("旋转偏移")]
        [SerializeField] private Vector3 _rotationOffset;

        [LabelText("缩放偏移")]
        [SerializeField] private Vector3 _scaleOffset;

        public GameObject TilePrefab => _tilePrefab;
        public Vector3 RotationOffset => _rotationOffset;
        public Vector3 ScaleOffset => _scaleOffset;
    }
}
