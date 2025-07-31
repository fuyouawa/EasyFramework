using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Core.Internal;
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
            [SerializeField] private bool[] _sudoku = new bool[9];
            [SerializeField] private TerrainRuleType _ruleType;

            public bool[] Sudoku => _sudoku;
            public TerrainRuleType RuleType => _ruleType;
        }

        [SerializeField] private List<RuleConfig> _ruleConfigs = new List<RuleConfig>();


        public TerrainRuleType GetRuleTypeBySudoku(bool[,] sudoku)
        {
            foreach (var ruleConfig in _ruleConfigs)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (ruleConfig.Sudoku[i * 3 + j] != sudoku[i, j])
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