using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.TileWorldPro;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public class BuiltInTileWorldDataStoreDrawer : EasyValueDrawer<BuiltInTileWorldDataStore>
    {
        private int? _tilesCount;
        private int? _bakedTilesCount;

        protected override void DrawProperty(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            EditorGUILayout.LabelField("区块数量", value.Chunks.Count.ToString());
            EditorGUILayout.LabelField("瓦片数量", _tilesCount == null ? "待计算..." : _tilesCount.Value.ToString());
            EditorGUILayout.LabelField("瓦片内存大小", $"{value.ChunkMemorySize / 1024f:F2} KB");

            if (GUILayout.Button("计算瓦片数量"))
            {
                _tilesCount = value.EnumerateChunks().Sum(chunk => chunk.CalculateTilesCount());
            }

            if (GUILayout.Button("删除空区块"))
            {
                var chunksToRemove = new List<ChunkPosition>();
                foreach (var chunk in value.Chunks)
                {
                    if (chunk.Value.CalculateTilesCount() == 0)
                    {
                        chunksToRemove.Add(chunk.Key);
                    }
                }

                foreach (var chunkPosition in chunksToRemove)
                {
                    value.RemoveChunk(chunkPosition);
                }
            }

            if (GUILayout.Button("删除所有区块"))
            {
                value.ClearAllChunks();
                _tilesCount = 0;
            }

            EasyEditorGUI.Title("烘焙数据");
            EditorGUILayout.LabelField("烘焙区块数量", value.BakedChunks.Count.ToString());
            EditorGUILayout.LabelField("烘焙瓦片数量", _bakedTilesCount == null ? "待计算..." : _bakedTilesCount.Value.ToString());
            EditorGUILayout.LabelField("烘焙瓦片内存大小", $"{value.BakedChunkMemorySize / 1024f:F2} KB");

            if (GUILayout.Button("计算烘焙瓦片数量"))
            {
                _bakedTilesCount = value.EnumerateBakedChunks().Sum(chunk => chunk.CalculateTilesCount());
            }

            if (GUILayout.Button("删除所有烘焙区块"))
            {
                value.ClearAllBakedChunks();
                _bakedTilesCount = 0;
            }
        }
    }
}