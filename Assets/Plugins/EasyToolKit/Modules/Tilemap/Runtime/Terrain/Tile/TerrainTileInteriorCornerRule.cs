using EasyToolKit.Inspector;
using System;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [MetroFoldoutGroup("内转角规则集", IconTextureGetter = "-t:EasyToolKit.Tilemap.Editor.TilemapEditorIcons -p:Instance.TerrainInteriorCornerTypeIcon")]
    [HideLabel]
    [Serializable]
    public class TerrainTileInteriorCornerRule : TerrainTileRuleBase
    {
        [LabelText("使用完整定义")]
        [SerializeField] private bool _useFullDefinition;

        [Space(3)]
        [TerrainRuleType(TerrainRuleType.TopLeftInteriorCorner)]
        [SerializeField] private TileObject _topLeftObject;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainRuleType(TerrainRuleType.TopRightInteriorCorner)]
        [SerializeField] private TileObject _topRightObject;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainRuleType(TerrainRuleType.BottomLeftInteriorCorner)]
        [SerializeField] private TileObject _bottomLeftObject;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainRuleType(TerrainRuleType.BottomRightInteriorCorner)]
        [SerializeField] private TileObject _bottomRightObject;

        public override TerrainTileRuleType RuleType => TerrainTileRuleType.InteriorCorner;

        /// <summary>
        /// 使用完整定义。
        /// 如果为 false，则使用<see cref="TopLeftObject"/>作为基准规则，其他方向的规则将基于此规则进行调整。
        /// </summary>
        public bool UseFullDefinition => _useFullDefinition;

        public TileObject TopLeftObject => _topLeftObject;
        public TileObject TopRightObject => _topRightObject;
        public TileObject BottomLeftObject => _bottomLeftObject;
        public TileObject BottomRightObject => _bottomRightObject;

        public override GameObject GetTileInstanceByRuleType(TerrainRuleType ruleType)
        {
            switch (ruleType)
            {
                case TerrainRuleType.TopLeftInteriorCorner:
                    return TopLeftObject.TryInstantiate();
                case TerrainRuleType.TopRightInteriorCorner:
                    {
                        if (UseFullDefinition)
                        {
                            return TopRightObject.TryInstantiate();
                        }

                        var instance = TopLeftObject.TryInstantiate();
                        if (instance == null)
                            return null;

                        instance.transform.rotation *= Quaternion.Euler(0, 90, 0);
                        return instance;
                    }
                case TerrainRuleType.BottomRightInteriorCorner:
                    {
                        if (UseFullDefinition)
                        {
                            return BottomRightObject.TryInstantiate();
                        }

                        var instance = TopLeftObject.TryInstantiate();
                        if (instance == null)
                            return null;

                        instance.transform.rotation *= Quaternion.Euler(0, 180, 0);
                        return instance;
                    }
                case TerrainRuleType.BottomLeftInteriorCorner:
                    {
                        if (UseFullDefinition)
                        {
                            return BottomLeftObject.TryInstantiate();
                        }

                        var instance = TopLeftObject.TryInstantiate();
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
