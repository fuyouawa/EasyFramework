using EasyToolKit.Inspector.Editor;
using EasyToolKit.Tilemap;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Tilemap.Editor
{
    [CustomEditor(typeof(TerrainConfigAsset))]
    public class TerrainConfigAssetEditor : EasyEditor
    {
    }

    public class TerrainConfigAsset_RuleConfigDrawer : EasyValueDrawer<TerrainConfigAsset.RuleConfig>
    {
        private InspectorProperty _ruleTypeProperty;

        protected override void Initialize()
        {
            _ruleTypeProperty = Property.Children["_ruleType"];
        }

        protected override void DrawProperty(GUIContent label)
        {
            var value = ValueEntry.SmartValue;

            EditorGUILayout.BeginVertical();

            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < 3; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < 3; j++)
                {
                    value.Sudoku[i * 3 + j] = EditorGUILayout.Toggle(value.Sudoku[i * 3 + j], GUILayout.Width(20), GUILayout.Height(20));
                }
                EditorGUILayout.EndHorizontal();
            }
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.Values.ForceMakeDirty();
            }

            _ruleTypeProperty.Draw();

            EditorGUILayout.EndVertical();
        }
    }
}