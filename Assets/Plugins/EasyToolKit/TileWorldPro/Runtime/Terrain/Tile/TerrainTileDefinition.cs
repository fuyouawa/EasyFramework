using EasyToolKit.Inspector;
using JetBrains.Annotations;
using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [Serializable]
    [HideLabel]
    public class TerrainTileDefinition
    {
        [LabelText("预制体")]
        [SerializeField] private GameObject _prefab;

        [LabelText("位置偏移")]
        [SerializeField] private Vector3 _positionOffset;

        [LabelText("旋转偏移")]
        [SerializeField] private Vector3 _rotationOffset;

        [LabelText("缩放偏移")]
        [SerializeField] private Vector3 _scaleOffset;

        public GameObject Prefab => _prefab;
        public Vector3 PositionOffset => _positionOffset;
        public Vector3 RotationOffset => _rotationOffset;
        public Vector3 ScaleOffset => _scaleOffset;

        [CanBeNull]
        public GameObject TryInstantiate()
        {
            if (_prefab == null)
            {
                return null;
            }

            var result = GameObject.Instantiate(Prefab);
            result.transform.position = PositionOffset;
            result.transform.rotation *= Quaternion.Euler(RotationOffset);
            result.transform.localScale += ScaleOffset;
            result.name = Prefab.name;
            return result;
        }
    }
}
