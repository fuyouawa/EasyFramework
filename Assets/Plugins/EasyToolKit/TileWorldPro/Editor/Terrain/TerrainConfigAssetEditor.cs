using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.TileWorldPro;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    [CustomEditor(typeof(TerrainConfigAsset))]
    public class TerrainConfigAssetEditor : EasyEditor
    {
        private static readonly GUIContent TempContent = new GUIContent();
        private static GUIStyle s_helpLabelStyle;

        private static GUIStyle HelpLabelStyle
        {
            get
            {
                if (s_helpLabelStyle == null)
                {
                    s_helpLabelStyle = new GUIStyle(EditorStyles.label)
                    {
                        fontSize = 13,
                        richText = true,
                        margin = new RectOffset(0, 0, 2, 0)
                    };
                }
                return s_helpLabelStyle;
            }
        }

        private static Texture s_emptyIcon;
        private static Texture s_containIcon;
        private static Texture s_optionIcon;
        private static Texture s_unionIcon;

        private static Texture EmptyIcon
        {
            get
            {
                if (s_emptyIcon == null)
                {
                    s_emptyIcon = TileWorldIcons.Instance.GetTerrainRuleGridTypeIcon(TerrainRuleGridType.Empty);
                }
                return s_emptyIcon;
            }
        }

        private static Texture ContainIcon
        {
            get
            {
                if (s_containIcon == null)
                {
                    s_containIcon = TileWorldIcons.Instance.GetTerrainRuleGridTypeIcon(TerrainRuleGridType.Contain);
                }

                return s_containIcon;
            }
        }

        private static Texture OptionIcon
        {
            get
            {
                if (s_optionIcon == null)
                {
                    s_optionIcon = TileWorldIcons.Instance.GetTerrainRuleGridTypeIcon(TerrainRuleGridType.Option);
                }
                return s_optionIcon;
            }
        }

        private static Texture UnionIcon
        {
            get
            {
                if (s_unionIcon == null)
                {
                    s_unionIcon = TileWorldIcons.Instance.GetTerrainRuleGridTypeIcon(TerrainRuleGridType.Union);
                }
                return s_unionIcon;
            }
        }

        private LocalPersistentContext<bool> _isHelpBoxExpanded;

        protected override void OnEnable()
        {
            base.OnEnable();
            _isHelpBoxExpanded = Tree.LogicRootProperty.GetPersistentContext(nameof(_isHelpBoxExpanded), false);
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw();

            EasyEditorGUI.BeginBox();
            EasyEditorGUI.BeginBoxHeader();

            _isHelpBoxExpanded.Value = EasyEditorGUI.Foldout(_isHelpBoxExpanded.Value, TempContent.SetText("帮助"));
            EasyEditorGUI.EndBoxHeader();

            if (_isHelpBoxExpanded.Value)
            {
                GUILayout.Label("通过九宫格的形式配置地形规则，格子中心是判定点（也就是当前瓦片的位置）。", HelpLabelStyle);

                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                GUI.DrawTexture(GUILayoutUtility.GetRect(20, 20, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)), EmptyIcon);
                GUILayout.Label("代表该格子不能有瓦片", HelpLabelStyle);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUI.DrawTexture(GUILayoutUtility.GetRect(20, 20, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)), ContainIcon);
                GUILayout.Label("代表该格子必须有瓦片", HelpLabelStyle);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUI.DrawTexture(GUILayoutUtility.GetRect(20, 20, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)), OptionIcon);
                GUILayout.Label("代表该格子不管有没有瓦片，都满足规则", HelpLabelStyle);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUI.DrawTexture(GUILayoutUtility.GetRect(20, 20, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)), UnionIcon);
                GUILayout.Label("代表该格子和其他相同标注的格子是联合判断的，要么都必须有瓦片，要么都不能有瓦片", HelpLabelStyle);
                EditorGUILayout.EndHorizontal();
            }

            EasyEditorGUI.EndBox();

            Tree.DrawProperties();
            Tree.EndDraw();
        }
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

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(100), GUILayout.ExpandHeight(false));
            for (int i = 0; i < 3; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < 3; j++)
                {
                    var index = i * 3 + j;
                    var gridType = value.Sudoku[index];
                    var icon = TileWorldIcons.Instance.GetTerrainRuleGridTypeIcon(gridType);

                    var buttonRect = GUILayoutUtility.GetRect(20, 20, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
                    if (GUI.Button(buttonRect, GUIContent.none))
                    {
                        gridType++;
                        if (gridType > TerrainRuleGridType.Union)
                        {
                            gridType = TerrainRuleGridType.Empty;
                        }
                        value.Sudoku[index] = gridType;
                        ValueEntry.Values.ForceMakeDirty();
                    }

                    if (Event.current.type == EventType.Repaint)
                    {
                        GUI.DrawTexture(buttonRect.AlignCenter(16, 16), icon);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            _ruleTypeProperty.Draw();

            EditorGUILayout.EndHorizontal();
        }
    }
}