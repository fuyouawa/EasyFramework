using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [EasyInspector]
    public class TerrainTileObject : MonoBehaviour
    {
        [SerializeField, ReadOnly] private TerrainObject _terrainObject;
        [SerializeField, ReadOnly] private TerrainTileRuleType _ruleType;
        [SerializeField, ReadOnly] private TilePosition _tilePosition;

        public TerrainObject TerrainObject { get => _terrainObject; set => _terrainObject = value; }
        public TerrainTileRuleType RuleType { get => _ruleType; set => _ruleType = value; }
        public TilePosition TilePosition { get => _tilePosition; set => _tilePosition = value; }
    }
}