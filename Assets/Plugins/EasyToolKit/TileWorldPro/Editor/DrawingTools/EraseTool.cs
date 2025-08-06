using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public class EraseTool : DraggableDrawingTool
    {
        protected override void DoTiles(TileWorldDesigner target, IEnumerable<TilePosition> tilePositions)
        {
            target.TileWorldAsset.RemoveTilesAt(tilePositions, TerrainDefinitionDrawer.SelectedGuid.Value);

            // target.DestroyTileAt(TerrainDefinitionDrawer.SelectedGuid.Value, tilePosition);

            // if (target.Asset.Settings.RealTimeIncrementalBuild)
            // {
            //     target.IncrementalBuildAt(TerrainDefinitionDrawer.SelectedGuid.Value, tilePosition);
            // }
        }

        protected override Color GetHitColor(TileWorldDesigner target)
        {
            return Color.red;
        }

        protected override bool FilterHitTile(TileWorldDesigner target, TilePosition tilePosition)
        {
            return target.TileWorldAsset.TerrainDefinitionSet.Contains(TerrainDefinitionDrawer.SelectedGuid.Value);
        }
    }
}