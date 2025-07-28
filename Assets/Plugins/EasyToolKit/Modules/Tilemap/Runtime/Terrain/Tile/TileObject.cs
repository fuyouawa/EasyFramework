using System;
using EasyToolKit.Inspector;
using JetBrains.Annotations;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [Serializable]
    [HideLabel]
    public class TileObject
    {
        [LabelText("预制体")]
        [SerializeField] private GameObject _prefab;

        [LabelText("旋转偏移")]
        [SerializeField] private Vector3 _rotationOffset;

        [LabelText("缩放偏移")]
        [SerializeField] private Vector3 _scaleOffset;

        public GameObject Prefab => _prefab;
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
            result.transform.rotation *= Quaternion.Euler(RotationOffset);
            result.transform.localScale += ScaleOffset;
            return result;
        }
    }
}
