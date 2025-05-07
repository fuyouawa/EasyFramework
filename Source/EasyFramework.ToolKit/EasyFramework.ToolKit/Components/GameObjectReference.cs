using System;
using EasyFramework.Core;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    [Serializable]
    public struct GameObjectReference
    {
        [SerializeField]
        private string _absolutePath;

        public string AbsolutePath => _absolutePath ?? string.Empty;

        public GameObjectReference(GameObject gameObject)
        {
            _absolutePath = gameObject?.transform.GetAbsolutePath();
        }

        public GameObject DeRef()
        {
            return GameObjectUtility.FindByAbsolutePath(_absolutePath);
        }
    }
}
