using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    public class WorldChunk
    {
        private class TerrainSection
        {
            public Guid TerrainGuid;
            public HashSet<WorldChunkTilePosition> Tiles;
        }

        private readonly WorldChunkArea _area;
        private readonly List<TerrainSection> _terrainSections;

        public WorldChunkArea Area => _area;

        public WorldChunk(WorldChunkArea area)
        {
            _area = area;
            _terrainSections = new List<TerrainSection>();
        }

        public Guid? TryGetTerrainGuidAt(TilePosition tilePosition)
        {
            var chunkTilePosition = _area.GetChunkTilePositionOf(tilePosition);
            foreach (var terrainSection in _terrainSections)
            {
                if (terrainSection.Tiles.Contains(chunkTilePosition))
                {
                    return terrainSection.TerrainGuid;
                }
            }
            return null;
        }

        public void SetTilesAt(IEnumerable<TilePosition> tilePositions, Guid terrainGuid)
        {
            var terrainSection = _terrainSections.FirstOrDefault(section => section.TerrainGuid == terrainGuid);
            if (terrainSection == null)
            {
                terrainSection = new TerrainSection
                {
                    TerrainGuid = terrainGuid,
                    Tiles = new HashSet<WorldChunkTilePosition>(),
                };
                _terrainSections.Add(terrainSection);
            }

            foreach (var tilePosition in tilePositions)
            {
                var chunkTilePosition = _area.GetChunkTilePositionOf(tilePosition);
                terrainSection.Tiles.Add(chunkTilePosition);
            }
        }

        public void RemoveTilesAt(IEnumerable<TilePosition> tilePositions, Guid terrainGuid)
        {
            var terrainSection = _terrainSections.FirstOrDefault(section => section.TerrainGuid == terrainGuid);
            if (terrainSection == null)
            {
                return;
            }

            foreach (var tilePosition in tilePositions)
            {
                var chunkTilePosition = _area.GetChunkTilePositionOf(tilePosition);
                terrainSection.Tiles.Remove(chunkTilePosition);
            }
        }

        public IEnumerable<TerrainTilePosition> EnumerateTiles()
        {
            foreach (var terrainSection in _terrainSections)
            {
                foreach (var tile in terrainSection.Tiles)
                {
                    yield return new TerrainTilePosition(tile.ConvertToTilePosition(), terrainSection.TerrainGuid);
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
                yield return new TerrainTilePosition(tile.ConvertToTilePosition(), terrainGuid);
            }
        }
    }
}