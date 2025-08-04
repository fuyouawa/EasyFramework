using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Tilemap.Editor
{
    public class EraseTool : DraggableDrawTool
    {
        protected override void DoTile(TilemapCreator target, Vector3Int tilePosition)
        {
            target.Asset.TerrainMap.RemoveTileAt(tilePosition);

            target.DestroyTileAt(TerrainDefinitionDrawer.SelectedGuid.Value, tilePosition);

            if (target.Asset.Settings.RealTimeIncrementalBuild)
            {
                target.IncrementalBuildAt(TerrainDefinitionDrawer.SelectedGuid.Value, tilePosition);
            }
        }

        protected override Color GetHitColor(TilemapCreator target)
        {
            return Color.red;
        }

        protected override bool FilterHitTile(TilemapCreator target, Vector3Int tilePosition)
        {
            var definition = target.Asset.TerrainMap.TryGetDefinitionAt(tilePosition);
            return definition != null && definition.Guid == TerrainDefinitionDrawer.SelectedGuid.Value;
        }
    }
}