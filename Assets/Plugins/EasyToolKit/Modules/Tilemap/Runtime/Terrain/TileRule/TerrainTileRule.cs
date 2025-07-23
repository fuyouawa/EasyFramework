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
        public GameObject TilePrefab;
        [LabelText("旋转偏移")]
        public Vector3 RotationOffset;
        [LabelText("缩放偏移")]
        public Vector3 ScaleOffset;
    }
}
