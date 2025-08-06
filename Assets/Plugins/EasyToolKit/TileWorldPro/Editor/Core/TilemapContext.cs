// using System;
// using System.Collections.Generic;
// using UnityEngine;

// namespace EasyToolKit.TileWorldPro.Editor
// {
//     [Serializable]
//     public class TilemapContext
//     {
//         [SerializeField]
//         private Dictionary<Vector3Int, TerrainTileRuleType> _ruleTypeMapCache =
//             new Dictionary<Vector3Int, TerrainTileRuleType>();

//         public TilemapCreator Target;
//         public bool IsMarkingRuleType = false;

//         public Dictionary<Vector3Int, TerrainTileRuleType> RuleTypeMapCache =>
//             _ruleTypeMapCache ??= new Dictionary<Vector3Int, TerrainTileRuleType>();
//     }
// }