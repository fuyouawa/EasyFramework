using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Tilemap.Editor
{
    public class TerrainTileRuleDrawer : EasyValueDrawer<TerrainTileRule>
    {
        public static readonly Color BackgroundColor = EditorGUIUtility.isProSkin
            ? new Color(0.216f, 0.216f, 0.216f, 1f)
            : new Color(0.801f, 0.801f, 0.801f, 1.000f);

        private InspectorProperty _tilePrefabProperty;
        private InspectorProperty _rotationOffsetProperty;
        private InspectorProperty _scaleOffsetProperty;

        protected override void Initialize()
        {
            _tilePrefabProperty = Property.Children[nameof(TerrainTileRule.TilePrefab)];
            _rotationOffsetProperty = Property.Children[nameof(TerrainTileRule.RotationOffset)];
            _scaleOffsetProperty = Property.Children[nameof(TerrainTileRule.ScaleOffset)];
        }

        protected override void DrawProperty(GUIContent label)
        {
            var type = Property.GetAttribute<TerrainTileRuleTypeAttribute>();
            if (type == null)
            {
                EditorGUILayout.HelpBox("TerrainTileRuleTypeAttribute is missing.", MessageType.Error);
                return;
            }

            var icon = TilemapEditorIcons.Instance.GetTerrainTypeIcon(type.TerrainType);

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

            _tilePrefabProperty.Draw();
            _rotationOffsetProperty.Draw();
            _scaleOffsetProperty.Draw();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }
    }
}
