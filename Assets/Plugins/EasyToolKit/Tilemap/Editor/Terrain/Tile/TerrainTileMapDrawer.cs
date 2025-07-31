using System;
using EasyToolKit.Inspector.Editor;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    public class TerrainTileMapDrawer : EasyValueDrawer<TerrainTileMap>
    {
        protected override void DrawProperty(GUIContent label)
        {
            ValueEntry.SmartValue.DefinitionsAsset.OnRemovedDefinition += OnRemovedDefinition;
            CallNextDrawer(label);
            ValueEntry.SmartValue.DefinitionsAsset.OnRemovedDefinition -= OnRemovedDefinition;

            if (GUILayout.Button("清除无效瓦片", GUILayout.Height(30)))
            {
                if (ValueEntry.SmartValue.ClearInvalidTiles())
                {
                    ValueEntry.Values.ForceMakeDirty();
                }
            }
        }

        private void OnRemovedDefinition(TerrainTileDefinition definition)
        {
            if (ValueEntry.SmartValue.ClearMatchedMap(definition.Guid))
            {
                ValueEntry.Values.ForceMakeDirty();
            }
        }
    }
}
