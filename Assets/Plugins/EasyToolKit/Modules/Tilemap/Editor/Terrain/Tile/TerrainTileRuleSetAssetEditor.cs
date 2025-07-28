using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Tilemap.Editor
{
    [CustomEditor(typeof(TerrainTileRuleSetAsset))]
    public class TerrainTileRuleSetAssetEditor : EasyEditor
    {
        private static readonly GUIContent TempContent = new GUIContent();
        private static readonly GUIContent TempContent2 = new GUIContent();

        protected override void DrawTree()
        {
            Tree.BeginDraw();

            if (!IsInlineEditor)
            {
                EditorGUILayout.LabelField(
                    TempContent.SetText("GUID"),
                    TempContent2.SetText(((TerrainTileRuleSetAsset)target).Guid.ToString("D")));

                MetroBoxGroupAttributeDrawer.BeginDraw(TempContent.SetText("地形瓦片规则集"), null);
            }

            Tree.DrawProperties();

            if (!IsInlineEditor)
            {
                MetroBoxGroupAttributeDrawer.EndDraw();
            }

            Tree.EndDraw();
        }
    }
}
