using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Tilemap.Editor
{
    [CustomEditor(typeof(TerrainRuleSetAsset))]
    public class TerrainRuleSetAssetEditor : EasyEditor
    {
        private static readonly GUIContent TempContent = new GUIContent();

        protected override void DrawTree()
        {
            Tree.BeginDraw();

            if (!IsInlineEditor)
            {
                MetroBoxGroupAttributeDrawer.BeginDraw(TempContent.SetText("地形规则集"), null);
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
