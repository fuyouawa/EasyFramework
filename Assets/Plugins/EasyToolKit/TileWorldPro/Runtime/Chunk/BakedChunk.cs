using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyToolKit.TileWorldPro
{
    public class BakedChunk
    {
        public class TerrainSection
        {
            public class Tile
            {
                private readonly ChunkTilePosition _tilePosition;
                private readonly TerrainTileRuleType _ruleType;
                private readonly bool _canBeHidden;

                public ChunkTilePosition TilePosition => _tilePosition;
                public TerrainTileRuleType RuleType => _ruleType;
                public bool CanBeHidden => _canBeHidden;

                public Tile(ChunkTilePosition tilePosition, TerrainTileRuleType ruleType, bool canBeHidden)
                {
                    _tilePosition = tilePosition;
                    _ruleType = ruleType;
                    _canBeHidden = canBeHidden;
                }
            }

            private readonly Guid _terrainGuid;
            private readonly Dictionary<ChunkTilePosition, Tile> _tiles;

            public Guid TerrainGuid => _terrainGuid;
            public IReadOnlyDictionary<ChunkTilePosition, Tile> Tiles => _tiles;

            public TerrainSection(Guid terrainGuid, IEnumerable<Tile> tiles)
            {
                _terrainGuid = terrainGuid;
                _tiles = new Dictionary<ChunkTilePosition, Tile>(tiles.ToDictionary(tile => tile.TilePosition));
            }
        }

        private readonly List<TerrainSection> _terrainSections;
        private readonly ChunkArea _area;

        public IReadOnlyList<TerrainSection> TerrainSections => _terrainSections;
        public ChunkArea Area => _area;

        public BakedChunk(ChunkArea area, IEnumerable<TerrainSection> terrainSections)
        {
            _area = area;
            _terrainSections = new List<TerrainSection>(terrainSections);
        }

        public int CalculateTilesCount()
        {
            return _terrainSections.Sum(section => section.Tiles.Count);
        }
    }
}