using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [EasyInspector]
    public class TerrainTileObject : MonoBehaviour
    {
        [SerializeField, ReadOnly] private TerrainObject _terrainObject;
        [SerializeField, ReadOnly] private TerrainTileRuleType _ruleType;
        [SerializeField, ReadOnly] private Vector3Int _tilePosition;

        public TerrainObject TerrainObject { get => _terrainObject; set => _terrainObject = value; }
        public TerrainTileRuleType RuleType { get => _ruleType; set => _ruleType = value; }
        public Vector3Int TilePosition { get => _tilePosition; set => _tilePosition = value; }
    }
}