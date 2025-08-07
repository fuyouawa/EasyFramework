using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [Serializable]
    public class ChunkTerrainTileInfo
    {
        [SerializeField] private GameObject _instance;
        [SerializeField] private ChunkTilePosition _chunkTilePosition;
        [SerializeField] private TerrainTileRuleType _ruleType;

        public ChunkTilePosition ChunkTilePosition => _chunkTilePosition;

        public GameObject Instance
        {
            get => _instance;
            set => _instance = value;
        }

        public TerrainTileRuleType RuleType
        {
            get => _ruleType;
            set => _ruleType = value;
        }

        public ChunkTerrainTileInfo(GameObject tileInstance, ChunkTilePosition chunkTilePosition, TerrainTileRuleType ruleType)
        {
            _instance = tileInstance;
            _chunkTilePosition = chunkTilePosition;
            _ruleType = ruleType;
        }
    }
}