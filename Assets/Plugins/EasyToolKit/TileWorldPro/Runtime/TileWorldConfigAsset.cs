using EasyToolKit.Core;
using EasyToolKit.Core.Internal;
using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [EasyInspector]
    [ModuleConfigsPath("TileWorldPro")]
    public class TileWorldConfigAsset : ScriptableObjectSingleton<TileWorldConfigAsset>
    {
        [LabelText("世界区块大小")]
        [SerializeField] private Vector3Int _worldChunkSize = new Vector3Int(16, 256, 16);

        public Vector3Int WorldChunkSize => _worldChunkSize;
    }
}