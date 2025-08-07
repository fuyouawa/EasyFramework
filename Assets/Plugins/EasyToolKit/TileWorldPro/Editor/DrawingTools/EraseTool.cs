using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public class EraseTool : DraggableDrawingTool, IEasyEventTrigger
    {
        protected override void DoTiles(TileWorldDesigner target, IEnumerable<TilePosition> tilePositions)
        {
            target.TileWorldAsset.RemoveTilesAt(tilePositions, SelectedTerrainDefinition.Guid);
            this.TriggerEvent(new RemoveTilesEvent(SelectedTerrainDefinition.Guid, tilePositions.ToArray()));
        }

        protected override Color GetHitColor(TileWorldDesigner target)
        {
            return Color.red;
        }

        protected override bool FilterHitTile(TileWorldDesigner target, TilePosition tilePosition)
        {
            return target.TileWorldAsset.TerrainDefinitionSet.Contains(SelectedTerrainDefinition.Guid);
        }
    }
}