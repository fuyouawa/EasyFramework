using EasyToolKit.Inspector;
using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [MetroFoldoutGroup("边缘规则集", IconTextureGetter = "-t:EasyToolKit.TileWorldPro.Editor.TileWorldIcons -p:Instance.TerrainEdgeTypeIcon")]
    [HideLabel]
    [Serializable]
    public class TerrainEdgeRule : TerrainRuleBase
    {
        [LabelText("使用完整定义")]
        [SerializeField] private bool _useFullDefinition;

        [Space(3)]
        [TerrainTileRuleType(TerrainTileRuleType.TopEdge)]
        [SerializeField] private TerrainTileDefinition _topTileDefinition;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainTileRuleType.BottomEdge)]
        [SerializeField] private TerrainTileDefinition _bottomTileDefinition;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainTileRuleType.LeftEdge)]
        [SerializeField] private TerrainTileDefinition _leftTileDefinition;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainTileRuleType.RightEdge)]
        [SerializeField] private TerrainTileDefinition _rightTileDefinition;

        public override TerrainRuleType RuleType => TerrainRuleType.Edge;

        /// <summary>
        /// 使用完整定义。
        /// 如果为 false，则使用<see cref="TopTileDefinition"/>作为基准规则，其他方向的规则将基于此规则进行调整。
        /// </summary>
        public bool UseFullDefinition => _useFullDefinition;

        public TerrainTileDefinition TopTileDefinition => _topTileDefinition;
        public TerrainTileDefinition BottomTileDefinition => _bottomTileDefinition;
        public TerrainTileDefinition LeftTileDefinition => _leftTileDefinition;
        public TerrainTileDefinition RightTileDefinition => _rightTileDefinition;


        public override GameObject GetTileInstanceByRuleType(TerrainTileRuleType ruleType)
        {
            switch (ruleType)
            {
                case TerrainTileRuleType.TopEdge:
                    return TopTileDefinition.TryInstantiate();
                case TerrainTileRuleType.RightEdge:
                    {
                        if (UseFullDefinition)
                        {
                            return RightTileDefinition.TryInstantiate();
                        }

                        var instance = TopTileDefinition.TryInstantiate();
                        if (instance == null)
                            return null;

                        instance.transform.rotation *= Quaternion.Euler(0, 90, 0);
                        return instance;
                    }
                case TerrainTileRuleType.BottomEdge:
                    {
                        if (UseFullDefinition)
                        {
                            return BottomTileDefinition.TryInstantiate();
                        }

                        var instance = TopTileDefinition.TryInstantiate();
                        if (instance == null)
                            return null;

                        instance.transform.rotation *= Quaternion.Euler(0, 180, 0);
                        return instance;
                    }
                case TerrainTileRuleType.LeftEdge:
                    {
                        if (UseFullDefinition)
                        {
                            return LeftTileDefinition.TryInstantiate();
                        }

                        var instance = TopTileDefinition.TryInstantiate();
                        if (instance == null)
                            return null;

                        instance.transform.rotation *= Quaternion.Euler(0, 270, 0);
                        return instance;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(ruleType), ruleType, null);
            }
        }
    }
}
