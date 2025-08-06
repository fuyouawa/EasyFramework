using EasyToolKit.Core;
using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [EasyInspector]
    public class TileWorldBuilder : MonoBehaviour
    {
        [LabelText("瓦片世界")]
        [SerializeField] private TileWorldAsset _tileWorldAsset;

        [LabelText("地形配置")]
        [SerializeField, InlineEditor(Style = InlineEditorStyle.Foldout)] private TerrainConfigAsset _terrainConfigAsset;

        public TileWorldAsset TileWorldAsset => _tileWorldAsset;

        private bool[,] GetSudokuAt(TilePosition tilePosition)
        {
            var sudoku = new bool[3, 3];
            var terrainGuid = _tileWorldAsset.TryGetTerrainGuidAt(tilePosition);
            Assert.IsNotNull(terrainGuid);

            sudoku[0, 2] = _tileWorldAsset.TryGetTerrainGuidAt(tilePosition + TilePosition.ForwardLeft) == terrainGuid;
            sudoku[1, 2] = _tileWorldAsset.TryGetTerrainGuidAt(tilePosition + TilePosition.Forward) == terrainGuid;
            sudoku[2, 2] = _tileWorldAsset.TryGetTerrainGuidAt(tilePosition + TilePosition.ForwardRight) == terrainGuid;

            sudoku[0, 1] = _tileWorldAsset.TryGetTerrainGuidAt(tilePosition + TilePosition.Left) == terrainGuid;
            sudoku[1, 1] = true;
            sudoku[2, 1] = _tileWorldAsset.TryGetTerrainGuidAt(tilePosition + TilePosition.Right) == terrainGuid;

            sudoku[0, 0] = _tileWorldAsset.TryGetTerrainGuidAt(tilePosition + TilePosition.BackLeft) == terrainGuid;
            sudoku[1, 0] = _tileWorldAsset.TryGetTerrainGuidAt(tilePosition + TilePosition.Back) == terrainGuid;
            sudoku[2, 0] = _tileWorldAsset.TryGetTerrainGuidAt(tilePosition + TilePosition.BackRight) == terrainGuid;

            return sudoku;
        }

        public TerrainTileRuleType CalculateRuleTypeOf(TilePosition tilePosition)
        {
            var sudoku = GetSudokuAt(tilePosition);
            return _terrainConfigAsset.GetRuleTypeBySudoku(sudoku);
        }
    }
}