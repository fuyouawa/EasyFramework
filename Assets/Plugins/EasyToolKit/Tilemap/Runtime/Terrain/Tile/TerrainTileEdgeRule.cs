using EasyToolKit.Inspector;
using System;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [MetroFoldoutGroup("边缘规则集", IconTextureGetter = "-t:EasyToolKit.Tilemap.Editor.TilemapEditorIcons -p:Instance.TerrainEdgeTypeIcon")]
    [HideLabel]
    [Serializable]
    public class TerrainTileEdgeRule : TerrainTileRuleBase
    {
        [LabelText("使用完整定义")]
        [SerializeField] private bool _useFullDefinition;

        [Space(3)]
        [TerrainRuleType(TerrainRuleType.TopEdge)]
        [SerializeField] private TileObject _topObject;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainRuleType(TerrainRuleType.BottomEdge)]
        [SerializeField] private TileObject _bottomObject;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainRuleType(TerrainRuleType.LeftEdge)]
        [SerializeField] private TileObject _leftObject;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainRuleType(TerrainRuleType.RightEdge)]
        [SerializeField] private TileObject _rightObject;

        public override TerrainTileRuleType RuleType => TerrainTileRuleType.Edge;

        /// <summary>
        /// 使用完整定义。
        /// 如果为 false，则使用<see cref="TopObject"/>作为基准规则，其他方向的规则将基于此规则进行调整。
        /// </summary>
        public bool UseFullDefinition => _useFullDefinition;

        public TileObject TopObject => _topObject;
        public TileObject BottomObject => _bottomObject;
        public TileObject LeftObject => _leftObject;
        public TileObject RightObject => _rightObject;


        public override GameObject GetTileInstanceByRuleType(TerrainRuleType ruleType)
        {
            switch (ruleType)
            {
                case TerrainRuleType.TopEdge:
                    return TopObject.TryInstantiate();
                case TerrainRuleType.LeftEdge:
                    {
                        if (UseFullDefinition)
                        {
                            return LeftObject.TryInstantiate();
                        }

                        var instance = TopObject.TryInstantiate();
                        if (instance == null)
                            return null;

                        instance.transform.rotation *= Quaternion.Euler(0, 90, 0);
                        return instance;
                    }
                case TerrainRuleType.BottomEdge:
                    {
                        if (UseFullDefinition)
                        {
                            return BottomObject.TryInstantiate();
                        }

                        var instance = TopObject.TryInstantiate();
                        if (instance == null)
                            return null;

                        instance.transform.rotation *= Quaternion.Euler(0, 180, 0);
                        return instance;
                    }
                case TerrainRuleType.RightEdge:
                    {
                        if (UseFullDefinition)
                        {
                            return RightObject.TryInstantiate();
                        }

                        var instance = TopObject.TryInstantiate();
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
