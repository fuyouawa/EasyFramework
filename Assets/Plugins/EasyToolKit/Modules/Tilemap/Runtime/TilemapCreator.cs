using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [EasyInspector]
    public class TilemapCreator : MonoBehaviour
    {
        [SerializeField, InlineEditor] private TilemapAsset _asset;
        
        private Camera _sceneViewCamera;
        private GameObject _target;
    }
}
