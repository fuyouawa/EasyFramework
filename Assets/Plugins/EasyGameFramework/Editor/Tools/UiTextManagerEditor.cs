using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace EasyGameFramework.Editor
{
    [CustomEditor(typeof(UiTextManager))]
    public class UiTextManagerEditor : OdinEditor
    {
        protected override void DrawTree()
        {
            base.DrawTree();

            var mgr = (UiTextManager)target;
            mgr.ApplyPresets();

            var fontAssetPreset = mgr.GetFontAssetPreset();
            if (fontAssetPreset != null)
            {
                EasyEditorGUI.FoldoutGroup(
                    new EasyEditorGUI.FoldoutGroupConfig(this, $"字体资产预设 - {fontAssetPreset.LabelToShow}")
                    {
                        Expandable = false,
                        OnContentGUI = rect =>
                        {
                            EditorGUI.BeginChangeCheck();
                            fontAssetPreset.FontAsset = (TMP_FontAsset)SirenixEditorFields.UnityObjectField(
                                new GUIContent("字体资产"), fontAssetPreset.FontAsset, typeof(TMP_FontAsset), false);
                            fontAssetPreset.Material =
                                (Material)SirenixEditorFields.UnityObjectField(new GUIContent("材质"),
                                    fontAssetPreset.Material, typeof(Material), false);
                            if (EditorGUI.EndChangeCheck())
                            {
                                EditorUtility.SetDirty(mgr);
                            }
                        }
                    });
            }

            var textPropertiesPreset = mgr.GetTextPropertiesPreset();
            if (textPropertiesPreset != null)
            {
                EasyEditorGUI.FoldoutGroup(
                    new EasyEditorGUI.FoldoutGroupConfig(this, $"文本属性预设 - {textPropertiesPreset.LabelToShow}")
                    {
                        Expandable = false,
                        OnContentGUI = rect =>
                        {
                            EditorGUI.BeginChangeCheck();
                            textPropertiesPreset.FontSize =
                                EditorGUILayout.FloatField("字体资产", textPropertiesPreset.FontSize);
                            textPropertiesPreset.FontColor = SirenixEditorFields.ColorField(new GUIContent("字体颜色"),
                                textPropertiesPreset.FontColor);
                            if (EditorGUI.EndChangeCheck())
                            {
                                EditorUtility.SetDirty(mgr);
                            }
                        }
                    });
            }

            if (SirenixEditorGUI.Button("切换预设管理器", ButtonSizes.Medium))
            {
                Selection.activeObject = UiTextPresetsManager.Instance;
                EditorGUIUtility.PingObject(UiTextPresetsManager.Instance);
            }
        }
    }
}
