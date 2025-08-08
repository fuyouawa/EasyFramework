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
        protected override void DoTiles(DrawingToolContext context, IReadOnlyList<TilePosition> tilePositions)
        {
            context.Target.TileWorldAsset.RemoveTilesAt(tilePositions, context.TerrainDefinition.Guid);
            this.TriggerEvent(new RemoveTilesEvent(context.TerrainDefinition.Guid, tilePositions.ToArray()));
        }

        protected override Color GetHitColor(DrawingToolContext context)
        {
            return Color.red;
        }

        protected override bool FilterHitTile(DrawingToolContext context, TilePosition tilePosition)
        {
            return context.Target.TileWorldAsset.TerrainDefinitionSet.Contains(context.TerrainDefinition.Guid);
        }
    }
}