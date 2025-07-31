using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Core.Internal;
using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [ModuleConfigsPath("Tilemap")]
    public class TerrainTileConfigAsset : ScriptableObjectSingleton<TerrainTileConfigAsset>
    {
        [Serializable]
        public class RuleConfig
        {
            [SerializeField, HideInInspector] private bool[] _sudoku = new bool[9];
            [SerializeField, HideLabel] private TerrainRuleType _ruleType;

            public bool[] Sudoku => _sudoku;
            public TerrainRuleType RuleType => _ruleType;
        }

        [MetroListDrawerSettings]
        [LabelText("规则配置表")]
        [SerializeField] private List<RuleConfig> _ruleConfigs = new List<RuleConfig>();


        public TerrainRuleType GetRuleTypeBySudoku(bool[,] sudoku)
        {
            bool[,] tempSudoku = new bool[3, 3];
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
                        if (tempSudoku[i, j] != sudoku[i, j])
                        {
                            goto NextRuleConfig;
                        }
                    }
                }
                return ruleConfig.RuleType;
            NextRuleConfig:;
            }
            return TerrainRuleType.Fill;
        }
    }
}