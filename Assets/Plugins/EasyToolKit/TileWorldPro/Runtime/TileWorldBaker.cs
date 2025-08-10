using System.Collections.Generic;
using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [EasyInspector]
    public class TileWorldBaker : MonoBehaviour
    {
        [FoldoutBoxGroup("设置")]
        [HideLabel]
        [SerializeField] private TileWorldBakerSettings _settings;

        [EndFoldoutBoxGroup]
        [LabelText("资产")]
        [InlineEditor]
        [SerializeField] private TileWorldAsset _tileWorldAsset;

        [LabelText("地形配置")]
        [InlineEditor(Style = InlineEditorStyle.Foldout)]
        [SerializeField] private TerrainConfigAsset _terrainConfigAsset;

        [Button("烘焙")]
        public void Bake()
        {
            _tileWorldAsset.DataStore.ClearAllBakedChunks();
            foreach (var chunk in _tileWorldAsset.EnumerateChunks())
            {
                var bakedTerrainSections = new List<BakedChunk.TerrainSection>();
                foreach (var terrainSection in chunk.TerrainSections)
                {
                    // var terrainDefinition = _tileWorldAsset.TerrainDefinitionSet.TryGetByGuid(terrainSection.TerrainGuid);
                    var bakedTiles = new List<BakedChunk.TerrainSection.Tile>();
                    foreach (var chunkTilePosition in terrainSection.Tiles)
                    {
                        var tilePosition = chunkTilePosition.ToTilePosition(chunk.Area);
                        var ruleType = _tileWorldAsset.CalculateRuleTypeOf(_terrainConfigAsset, tilePosition);
                        //TODO hide detection
                        var bakedTile = new BakedChunk.TerrainSection.Tile(chunkTilePosition, ruleType, false);
                        bakedTiles.Add(bakedTile);
                    }
                    var bakedTerrainSection = new BakedChunk.TerrainSection(terrainSection.TerrainGuid, bakedTiles);
                    bakedTerrainSections.Add(bakedTerrainSection);
                }
                var bakedChunk = new BakedChunk(chunk.Area, bakedTerrainSections);
                _tileWorldAsset.DataStore.UpdateBakedChunk(bakedChunk);
            }
        }
    }
}