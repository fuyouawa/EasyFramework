using EasyToolKit.Inspector;
using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [MetroFoldoutGroup("外转角规则集", IconTextureGetter = "-t:EasyToolKit.TileWorldPro.Editor.TileWorldIcons -p:Instance.TerrainExteriorCornerTypeIcon")]
    [HideLabel]
    [Serializable]
    public class TerrainExteriorCornerRule : TerrainRuleBase
    {
        [LabelText("使用完整定义")]
        [SerializeField] private bool _useFullDefinition;

        [Space(3)]
        [TerrainTileRuleType(TerrainTileRuleType.TopLeftExteriorCorner)]
        [SerializeField] private TerrainTileDefinition _topLeftTileDefinition;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainTileRuleType.TopRightExteriorCorner)]
        [SerializeField] private TerrainTileDefinition _topRightTileDefinition;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainTileRuleType.BottomLeftExteriorCorner)]
        [SerializeField] private TerrainTileDefinition _bottomLeftTileDefinition;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainTileRuleType.BottomRightExteriorCorner)]
        [SerializeField] private TerrainTileDefinition _bottomRightTileDefinition;

        public override TerrainRuleType RuleType => TerrainRuleType.ExteriorCorner;

        /// <summary>
        /// 使用完整定义。
        /// 如果为 false，则使用<see cref="TopLeftTileDefinition"/>作为基准规则，其他方向的规则将基于此规则进行调整。
        /// </summary>
        public bool UseFullDefinition => _useFullDefinition;

        public TerrainTileDefinition TopLeftTileDefinition => _topLeftTileDefinition;
        public TerrainTileDefinition TopRightTileDefinition => _topRightTileDefinition;
        public TerrainTileDefinition BottomLeftTileDefinition => _bottomLeftTileDefinition;
        public TerrainTileDefinition BottomRightTileDefinition => _bottomRightTileDefinition;


        public override GameObject GetTileInstanceByRuleType(TerrainTileRuleType ruleType)
        {
            switch (ruleType)
            {
                case TerrainTileRuleType.TopLeftExteriorCorner:
                    return TopLeftTileDefinition.TryInstantiate();
                case TerrainTileRuleType.TopRightExteriorCorner:
                    {
                        if (UseFullDefinition)
                        {
                            return TopRightTileDefinition.TryInstantiate();
                        }

                        var instance = TopLeftTileDefinition.TryInstantiate();
                        if (instance == null)
                            return null;

                        instance.transform.rotation *= Quaternion.Euler(0, 90, 0);
                        return instance;
                    }
                case TerrainTileRuleType.BottomRightExteriorCorner:
                    {
                        if (UseFullDefinition)
                        {
                            return BottomRightTileDefinition.TryInstantiate();
                        }

                        var instance = TopLeftTileDefinition.TryInstantiate();
                        if (instance == null)
                            return null;

                        instance.transform.rotation *= Quaternion.Euler(0, 180, 0);
                        return instance;
                    }
                case TerrainTileRuleType.BottomLeftExteriorCorner:
                    {
                        if (UseFullDefinition)
                        {
                            return BottomLeftTileDefinition.TryInstantiate();
                        }

                        var instance = TopLeftTileDefinition.TryInstantiate();
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
