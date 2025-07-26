using System;
using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    public class TilemapCreator : MonoBehaviour
    {
        [SerializeField, InlineEditor] private TilemapAsset _asset;
        
        private Camera _sceneViewCamera;
        private GameObject _target;

        public TilemapAsset Asset => _asset;
    }
}
