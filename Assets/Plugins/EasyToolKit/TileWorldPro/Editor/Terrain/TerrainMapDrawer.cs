// using System;
// using EasyToolKit.Inspector.Editor;
// using UnityEngine;

// namespace EasyToolKit.TileWorldPro
// {
//     public class TerrainMapDrawer : EasyValueDrawer<TerrainMap>
//     {
//         protected override void DrawProperty(GUIContent label)
//         {
//             ValueEntry.SmartValue.DefinitionSet.OnRemovedDefinition += OnRemovedDefinition;
//             CallNextDrawer(label);
//             ValueEntry.SmartValue.DefinitionSet.OnRemovedDefinition -= OnRemovedDefinition;

//             if (GUILayout.Button("清除无效瓦片", GUILayout.Height(30)))
//             {
//                 if (ValueEntry.SmartValue.ClearInvalidTiles())
//                 {
//                     ValueEntry.Values.ForceMakeDirty();
//                 }
//             }
//         }

//         private void OnRemovedDefinition(TerrainDefinition definition)
//         {
//             if (ValueEntry.SmartValue.ClearMatchedMap(definition.Guid))
//             {
//                 ValueEntry.Values.ForceMakeDirty();
//             }
//         }
//     }
// }
