using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public class TerrainTileDefinitionDrawer : EasyValueDrawer<TerrainTileDefinition>
    {
        public static readonly Color BackgroundColor = EditorGUIUtility.isProSkin
            ? new Color(0.216f * 0.9f, 0.216f * 0.9f, 0.216f * 0.9f, 1f)
            : new Color(0.801f, 0.801f, 0.801f, 1.000f);

        protected override void DrawProperty(GUIContent label)
        {
            var type = Property.GetAttribute<TerrainTileRuleTypeAttribute>();
            if (type == null)
            {
                EasyEditorGUI.MessageBox("TerrainTileRuleTypeAttribute is missing.", MessageType.Error);
                return;
            }

            var icon = TileWorldIcons.Instance.GetTerrainTileTypeIcon(type.RuleType);

            var totalRect = EditorGUILayout.BeginHorizontal();
            totalRect.xMin += EasyGUIHelper.CurrentIndentAmount;
            totalRect.xMax += EasyGUIHelper.CurrentIndentAmount;
            EasyEditorGUI.DrawSolidRect(totalRect, BackgroundColor);

            var iconRect = GUILayoutUtility.GetRect(64, 64, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
            iconRect = iconRect.AlignCenter(32, 32);
            iconRect.xMin += EasyGUIHelper.CurrentIndentAmount;
            iconRect.xMax += EasyGUIHelper.CurrentIndentAmount;

            GUI.DrawTexture(iconRect, icon);

            EditorGUILayout.BeginVertical();

            for (int i = 0; i < Property.Children.Count; i++)
            {
                Property.Children[i].Draw();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }
    }
}
