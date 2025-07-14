using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    public class TilemapCreator : MonoBehaviour
    {
        [SerializeField] private TilemapAsset _asset;
        
        private Camera _sceneViewCamera;
        private GameObject _target;
    }
}
