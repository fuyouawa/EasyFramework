using System.Linq;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.TileWorldPro;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public class BuiltInTileWorldDataStoreDrawer : EasyValueDrawer<BuiltInTileWorldDataStore>
    {
        private int? _tilesCount;

        protected override void DrawProperty(GUIContent label)
        {
            EditorGUILayout.LabelField("区块数量", ValueEntry.SmartValue.Chunks.Count.ToString());
            EditorGUILayout.LabelField("瓦片数量", _tilesCount == null ? "待计算..." : _tilesCount.Value.ToString());

            if (GUILayout.Button("计算瓦片数量"))
            {
                _tilesCount = ValueEntry.SmartValue.EnumerateChunks().Sum(chunk => chunk.CalculateTilesCount());
            }
        }
    }
}