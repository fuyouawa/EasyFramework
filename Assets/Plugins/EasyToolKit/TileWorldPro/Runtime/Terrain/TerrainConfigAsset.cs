using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Core.Internal;
using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    public enum TerrainRuleGridType
    {
        Empty,
        Contain,
        Option,
        Union
    }

    [CreateAssetMenu(fileName = "TerrainConfig", menuName = "EasyToolKit/TileWorldPro/TerrainConfig")]
    public class TerrainConfigAsset : ScriptableObject
    {
        [Serializable]
        public class RuleConfig
        {
            [SerializeField, HideInInspector] private TerrainRuleGridType[] _sudoku = new TerrainRuleGridType[9];
            [SerializeField, HideLabel] private TerrainTileRuleType _ruleType;

            public TerrainRuleGridType[] Sudoku => _sudoku;
            public TerrainTileRuleType RuleType => _ruleType;
        }

        [MetroListDrawerSettings]
        [LabelText("规则配置表")]
        [SerializeField] private List<RuleConfig> _ruleConfigs = new List<RuleConfig>();


        public TerrainTileRuleType GetRuleTypeBySudoku(bool[,] sudoku)
        {
            TerrainRuleGridType[,] tempSudoku = new TerrainRuleGridType[3, 3];
            bool? firstUnion = null;
            foreach (var ruleConfig in _ruleConfigs)
            {
                tempSudoku[0, 2] = ruleConfig.Sudoku[0];
                tempSudoku[1, 2] = ruleConfig.Sudoku[1];
                tempSudoku[2, 2] = ruleConfig.Sudoku[2];

                tempSudoku[0, 1] = ruleConfig.Sudoku[3];
                tempSudoku[1, 1] = ruleConfig.Sudoku[4];
                tempSudoku[2, 1] = ruleConfig.Sudoku[5];

                tempSudoku[0, 0] = ruleConfig.Sudoku[6];
                tempSudoku[1, 0] = ruleConfig.Sudoku[7];
                tempSudoku[2, 0] = ruleConfig.Sudoku[8];

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        switch (tempSudoku[i, j])
                        {
                            case TerrainRuleGridType.Empty:
                                if (sudoku[i, j])
                                {
                                    goto NextRuleConfig;
                                }
                                break;
                            case TerrainRuleGridType.Contain:
                                if (!sudoku[i, j])
                                {
                                    goto NextRuleConfig;
                                }
                                break;
                            case TerrainRuleGridType.Option:
                                break;
                            case TerrainRuleGridType.Union:
                                if (firstUnion == null)
                                {
                                    firstUnion = sudoku[i, j];
                                }
                                else if (firstUnion != sudoku[i, j])
                                {
                                    goto NextRuleConfig;
                                }
                                break;
                        }
                    }
                }
                return ruleConfig.RuleType;
            NextRuleConfig:;
            }
            return TerrainTileRuleType.Fill;
        }
    }
}
