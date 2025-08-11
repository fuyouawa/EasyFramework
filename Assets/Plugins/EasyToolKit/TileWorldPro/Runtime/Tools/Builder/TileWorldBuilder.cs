using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EasyToolKit.TileWorldPro
{
    public class TileWorldBuilder : SerializedMonoBehaviour
    {
        [LabelText("起始点")]
        [SerializeField] private TileWorldStartPoint _startPoint;

        [FoldoutBoxGroup("设置")]
        [HideLabel]
        [SerializeField] private TileWorldBuilderSettings _settings;

        [EndFoldoutBoxGroup]
        [LabelText("资产")]
        [InlineEditor(Style = InlineEditorStyle.FoldoutBox)]
        [SerializeField] private TileWorldAsset _tileWorldAsset;

        [LabelText("地形配置")]
        [InlineEditor(Style = InlineEditorStyle.FoldoutBox)]
        [SerializeField] private TerrainConfigAsset _terrainConfigAsset;

        [HideLabel]
        [OdinSerialize, ShowInInspector] private TileBuildPipline _tileBuildPipline;

        private Dictionary<ChunkPosition, ChunkObject> _chunkObjects;

        public TileWorldStartPoint StartPoint => _startPoint;

        public TileWorldBuilderSettings Settings => _settings;

        public TileWorldAsset TileWorldAsset => _tileWorldAsset;

        public IReadOnlyDictionary<ChunkPosition, ChunkObject> ChunkObjects
        {
            get
            {
                if (_chunkObjects == null)
                {
                    _chunkObjects = transform.GetComponentsInChildren<ChunkObject>(true).ToDictionary(chunkObject => chunkObject.Area.Position);
                }
                return _chunkObjects;
            }
        }

        [Title("构建操作", TextAlignment = TextAlignment.Center)]
        [Button("重新构建")]
        public void RebuildAll()
        {
            ClearAll();
            BuildAll();
        }

        private void BuildAll()
        {
            foreach (var terrainDefinition in _tileWorldAsset.TerrainDefinitionSet)
            {
                BuildTerrain(terrainDefinition.Guid);
            }
        }

        public void RebuildTerrain(Guid terrainGuid)
        {
            ClearTerrain(terrainGuid);
            BuildTerrain(terrainGuid);
        }

        private void BuildTerrain(Guid terrainGuid)
        {
            var tilePositions = _tileWorldAsset
                    .EnumerateChunks()
                    .SelectMany(chunk => chunk.EnumerateTerrainTiles(terrainGuid))
                    .Select(tile => tile.TilePosition)
                    .ToArray();
            BuildTiles(terrainGuid, tilePositions, false);
        }

        public ChunkObject GetChunkObjectOf(ChunkPosition chunkPosition)
        {
            if (ChunkObjects.TryGetValue(chunkPosition, out var chunkObject) && chunkObject != null)
            {
                return chunkObject;
            }

            chunkObject = new GameObject($"Chunk_{chunkPosition}").AddComponent<ChunkObject>();
            chunkObject.transform.SetParent(transform);
            chunkObject.Initialize(this, new ChunkArea(chunkPosition, _tileWorldAsset.ChunkSize));
            _chunkObjects[chunkPosition] = chunkObject;
            return chunkObject;
        }

        public ChunkTerrainTileInfo TryGetTileInfoOf(Guid terrainGuid, TilePosition tilePosition)
        {
            var chunkObject = GetChunkObjectOf(tilePosition.ToChunkPosition(_tileWorldAsset.ChunkSize));
            var chunkTilePosition = chunkObject.Area.TilePositionToChunkTilePosition(tilePosition);
            return chunkObject.GetTerrainObject(terrainGuid).TryGetTileInfoOf(chunkTilePosition);
        }

        [Button("清空构建瓦片")]
        public void ClearAll(bool destroyChunkObject = true)
        {
            foreach (var chunkObject in ChunkObjects.Values)
            {
                if (destroyChunkObject)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(chunkObject.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(chunkObject.gameObject);
                    }
                }
                else
                {
                    chunkObject.ClearAll(false);
                }
            }

            if (destroyChunkObject)
            {
                _chunkObjects.Clear();
            }
        }

        public void ClearTerrain(Guid terrainGuid, bool destroyTerrainObject = true)
        {
            foreach (var chunkObject in ChunkObjects.Values)
            {
                chunkObject.ClearTerrain(terrainGuid, destroyTerrainObject);
            }
        }

        public void RebuildTiles(Guid terrainGuid, IReadOnlyList<TilePosition> tilePositions, bool rebuildAffectedTilesIfNeeded = true)
        {
            DestroyTiles(terrainGuid, tilePositions);
            BuildTiles(terrainGuid, tilePositions, rebuildAffectedTilesIfNeeded);
        }

        private void BuildTiles(Guid terrainGuid, IReadOnlyList<TilePosition> tilePositions, bool rebuildAffectedTilesIfNeeded = true)
        {
            ChunkObject currentChunkObject = null;
            foreach (var tilePosition in tilePositions)
            {
                if (currentChunkObject == null || !currentChunkObject.Area.Contains(tilePosition))
                {
                    currentChunkObject = GetChunkObjectOf(tilePosition.ToChunkPosition(_tileWorldAsset.ChunkSize));
                }

                InstantiateTileAndAddToChunk(currentChunkObject, terrainGuid, tilePosition);
            }

            if (rebuildAffectedTilesIfNeeded)
            {
                RebuildAffectedTilesIfNeeded(terrainGuid, tilePositions);
            }
        }

        public void DestroyTiles(Guid terrainGuid, IReadOnlyList<TilePosition> tilePositions, bool rebuildAffectedTilesIfNeeded = true)
        {
            ChunkObject currentChunkObject = null;
            foreach (var tilePosition in tilePositions)
            {
                if (currentChunkObject == null || !currentChunkObject.Area.Contains(tilePosition))
                {
                    currentChunkObject = GetChunkObjectOf(tilePosition.ToChunkPosition(_tileWorldAsset.ChunkSize));
                }

                var chunkTilePosition = currentChunkObject.Area.TilePositionToChunkTilePosition(tilePosition);
                currentChunkObject.DestroyTile(terrainGuid, chunkTilePosition);
            }

            if (rebuildAffectedTilesIfNeeded)
            {
                RebuildAffectedTilesIfNeeded(terrainGuid, tilePositions);
            }
        }

        private void RebuildAffectedTilesIfNeeded(Guid terrainGuid, IReadOnlyList<TilePosition> tilePositions, List<TilePosition> ignoredAffectedTilePositions = null)
        {
            var affectedTilePositions = new HashSet<TilePosition>();
            foreach (var tilePosition in tilePositions)
            {
                var position = tilePosition + TilePosition.ForwardLeft;
                if (IsValidTilePosition(position)) affectedTilePositions.Add(position);
                position = tilePosition + TilePosition.Forward;
                if (IsValidTilePosition(position)) affectedTilePositions.Add(position);
                position = tilePosition + TilePosition.ForwardRight;
                if (IsValidTilePosition(position)) affectedTilePositions.Add(position);

                position = tilePosition + TilePosition.Left;
                if (IsValidTilePosition(position)) affectedTilePositions.Add(position);
                position = tilePosition + TilePosition.Right;
                if (IsValidTilePosition(position)) affectedTilePositions.Add(position);

                position = tilePosition + TilePosition.BackLeft;
                if (IsValidTilePosition(position)) affectedTilePositions.Add(position);
                position = tilePosition + TilePosition.Back;
                if (IsValidTilePosition(position)) affectedTilePositions.Add(position);
                position = tilePosition + TilePosition.BackRight;
                if (IsValidTilePosition(position)) affectedTilePositions.Add(position);
            }

            foreach (var tilePosition in tilePositions)
            {
                affectedTilePositions.Remove(tilePosition);
            }

            if (ignoredAffectedTilePositions != null)
            {
                foreach (var tilePosition in ignoredAffectedTilePositions)
                {
                    affectedTilePositions.Remove(tilePosition);
                }
            }

            ChunkObject currentChunkObject = null;
            ChunkTerrainObject currentTerrainObject = null;

            var changedTilePositions = new List<TilePosition>();
            foreach (var tilePosition in affectedTilePositions)
            {
                if (currentChunkObject == null || !currentChunkObject.Area.Contains(tilePosition))
                {
                    currentChunkObject = GetChunkObjectOf(tilePosition.ToChunkPosition(_tileWorldAsset.ChunkSize));
                    currentTerrainObject = currentChunkObject.GetTerrainObject(terrainGuid);
                }

                var chunkTilePosition = currentChunkObject.Area.TilePositionToChunkTilePosition(tilePosition);
                var tileInfo = currentTerrainObject.TryGetTileInfoOf(chunkTilePosition);
                if (tileInfo == null)
                {
                    continue;
                }

                var ruleType = _tileWorldAsset.CalculateRuleTypeOf(_terrainConfigAsset, tilePosition);
                if (tileInfo.RuleType == ruleType)
                {
                    continue;
                }

                currentTerrainObject.DestroyTileAt(chunkTilePosition);
                InstantiateTileAndAddToChunk(currentChunkObject, terrainGuid, chunkTilePosition, ruleType);
                changedTilePositions.Add(tilePosition);
            }

            if (changedTilePositions.Count > 0)
            {
                if (ignoredAffectedTilePositions == null)
                {
                    ignoredAffectedTilePositions = new List<TilePosition>();
                }
                ignoredAffectedTilePositions.AddRange(tilePositions);
                RebuildAffectedTilesIfNeeded(terrainGuid, changedTilePositions, ignoredAffectedTilePositions);
            }
        }

        private void InstantiateTileAndAddToChunk(ChunkObject chunkObject, Guid terrainGuid, TilePosition tilePosition)
        {
            var ruleType = _tileWorldAsset.CalculateRuleTypeOf(_terrainConfigAsset, tilePosition);
            var chunkTilePosition = chunkObject.Area.TilePositionToChunkTilePosition(tilePosition);
            InstantiateTileAndAddToChunk(chunkObject, terrainGuid, chunkTilePosition, ruleType);
        }

        private void InstantiateTileAndAddToChunk(ChunkObject chunkObject, Guid terrainGuid, ChunkTilePosition chunkTilePosition, TerrainTileRuleType ruleType)
        {
            var beforeTileInstantiateParameters = new BeforeTileInstantiateParameters(this, chunkObject, terrainGuid, chunkTilePosition, ruleType);
            _tileBuildPipline.BeforeInstantiateTile(beforeTileInstantiateParameters);
            terrainGuid = beforeTileInstantiateParameters.TerrainGuid;
            chunkTilePosition = beforeTileInstantiateParameters.ChunkTilePosition;
            ruleType = beforeTileInstantiateParameters.RuleType;

            var tileInstance = _tileWorldAsset.TerrainDefinitionSet.TryGetByGuid(terrainGuid).RuleSetAsset.GetTileInstanceByRuleType(ruleType);
            if (tileInstance == null)
            {
                Debug.LogError($"The Rule Type '{ruleType}' of tile instance is null for tile position '{chunkTilePosition}'");
                return;
            }

            var afterTileInstantiateParameters = new AfterTileInstantiateParameters(this, chunkObject, tileInstance);
            _tileBuildPipline.AfterInstantiateTile(afterTileInstantiateParameters);
            tileInstance = afterTileInstantiateParameters.TileInstance;

            chunkObject.AddTile(tileInstance, terrainGuid, chunkTilePosition, ruleType);
        }


        [Title("烘培操作", TextAlignment = TextAlignment.Center)]
        [Button("烘焙瓦片数据")]
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


        private bool IsValidTilePosition(TilePosition tilePosition)
        {
            if (tilePosition.X < 0 ||
                tilePosition.Y < 0 ||
                tilePosition.Z < 0)
            {
                return false;
            }
            return true;
        }
    }
}