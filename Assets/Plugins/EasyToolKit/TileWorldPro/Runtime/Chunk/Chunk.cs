using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    public class Chunk
    {
        public class TerrainSection
        {
            private Guid _terrainGuid;
            private HashSet<ChunkTilePosition> _tiles;
            private TerrainDefinition _terrainDefinitionCache;

            public Guid TerrainGuid => _terrainGuid;
            public HashSet<ChunkTilePosition> Tiles => _tiles;

            public TerrainSection(Guid terrainGuid)
            {
                _terrainGuid = terrainGuid;
                _tiles = new HashSet<ChunkTilePosition>();
            }

            public TerrainSection(Guid terrainGuid, IEnumerable<ChunkTilePosition> initialTiles)
            {
                _terrainGuid = terrainGuid;
                _tiles = new HashSet<ChunkTilePosition>(initialTiles);
            }

            public TerrainDefinition GetTerrainDefinitionCached(TerrainDefinitionSet terrainDefinitionSet)
            {
                if (_terrainDefinitionCache == null)
                {
                    _terrainDefinitionCache = terrainDefinitionSet.TryGetByGuid(_terrainGuid);
                }
                return _terrainDefinitionCache;
            }
        }

        private readonly ChunkArea _area;
        private readonly List<TerrainSection> _terrainSections;

        public ChunkArea Area => _area;
        public IReadOnlyList<TerrainSection> TerrainSections => _terrainSections;

        public Chunk(ChunkArea area)
        {
            _area = area;
            _terrainSections = new List<TerrainSection>();
        }

        public Chunk(ChunkArea area, IEnumerable<TerrainSection> initialTerrainSections)
        {
            _area = area;
            _terrainSections = new List<TerrainSection>(initialTerrainSections);
        }

        public Guid? TryGetTerrainGuidAt(TilePosition tilePosition)
        {
            var chunkTilePosition = _area.TilePositionToChunkTilePosition(tilePosition);
            foreach (var terrainSection in _terrainSections)
            {
                if (terrainSection.Tiles.Contains(chunkTilePosition))
                {
                    return terrainSection.TerrainGuid;
                }
            }
            return null;
        }

        public void SetTilesAt(IReadOnlyList<ChunkTilePosition> chunkTilePositions, Guid terrainGuid)
        {
            var terrainSection = _terrainSections.FirstOrDefault(section => section.TerrainGuid == terrainGuid);
            if (terrainSection == null)
            {
                terrainSection = new TerrainSection(terrainGuid);
                _terrainSections.Add(terrainSection);
            }

            foreach (var chunkTilePosition in chunkTilePositions)
            {
                terrainSection.Tiles.Add(chunkTilePosition);
            }
        }

        public void RemoveTilesAt(IReadOnlyList<ChunkTilePosition> chunkTilePositions, Guid terrainGuid)
        {
            var terrainSection = _terrainSections.FirstOrDefault(section => section.TerrainGuid == terrainGuid);
            if (terrainSection == null)
            {
                return;
            }

            foreach (var chunkTilePosition in chunkTilePositions)
            {
                terrainSection.Tiles.Remove(chunkTilePosition);
            }
        }

        public int CalculateTilesCount()
        {
            return _terrainSections.Sum(section => section.Tiles.Count);
        }

        public IEnumerable<TerrainTilePosition> EnumerateTiles()
        {
            foreach (var terrainSection in _terrainSections)
            {
                foreach (var tile in terrainSection.Tiles)
                {
                    yield return new TerrainTilePosition(tile.ToTilePosition(_area), terrainSection.TerrainGuid);
                }
            }
        }

        public IEnumerable<TerrainTilePosition> EnumerateTerrainTiles(Guid terrainGuid)
        {
            var terrainSection = _terrainSections.FirstOrDefault(section => section.TerrainGuid == terrainGuid);
            if (terrainSection == null)
            {
                yield break;
            }

            foreach (var tile in terrainSection.Tiles)
            {
                yield return new TerrainTilePosition(tile.ToTilePosition(_area), terrainGuid);
            }
        }
    }
}