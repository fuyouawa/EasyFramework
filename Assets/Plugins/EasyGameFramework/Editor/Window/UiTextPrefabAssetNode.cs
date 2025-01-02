using Pokemon.UI;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using EasyFramework;
using JetBrains.Annotations;

namespace EasyGameFramework.Editor
{
    [Serializable]
    [HideReferenceObjectPicker]
    public class UiTextPrefabAssetNode
    {
        [ReadOnly]
        [LabelText("完整名称")]
        public string FullName;

        [HideInInspector]
        public string AssetPath;

        [HideInInspector]
        public bool Checked;

        public GameObject Prefab
        {
            get
            {
                var ps = PrefabStageUtility.GetCurrentPrefabStage();
                if (ps != null && ps.assetPath == AssetPath)
                {
                    return ps.prefabContentsRoot;
                }

                return AssetDatabase.LoadAssetAtPath<GameObject>(AssetPath);
            }
        }

        public Transform Node => Prefab.transform.Find(FullName) ??
                                   throw new Exception($"有节点被删除({AssetPath} -> {FullName}), 需要重新加载\"UI文本组件视图\"中的\"组件列表!\"");

        public UiTextPrefabAssetNode(string fullName, string assetPath)
        {
            AssetPath = assetPath;
            FullName = fullName;
        }

        [InfoBoxCN("@TextManagerInfo", VisibleIf = "@!HasNotManager()")]
        [InfoBoxCN("该UI文本组件没有被UITextManager管理", InfoMessageType.Warning, VisibleIf = "HasNotManager")]
        [InfoBoxCN("该UI文本组件有多个重复的UITextManager", InfoMessageType.Warning, VisibleIf = "HasRepeatedManager")]
        [Button("在Hierarchy中选中该物体")]
        [UsedImplicitly]
        private void Select()
        {
            var ps = PrefabStageUtility.OpenPrefab(AssetPath);
            var target = ps.prefabContentsRoot.transform.Find(FullName);
            Debug.Assert(target != null);
            Selection.activeObject = target;
            EditorGUIUtility.PingObject(target);
        }

        public string Label => FullName;

        public string TextManagerInfo
        {
            get
            {
                var mgr = Node.GetComponent<UITextManager>();
                var n1 = mgr.FontAssetPreset?.Label;
                var n2 = mgr.TextPropertiesPreset?.Label;
                return $"该UI文本组件使用的预设为\n" +
                       $"字体资产预设:{n1.DefaultIfNullOrEmpty("无")}\n" +
                       $"Text属性预设:{n2.DefaultIfNullOrEmpty("无")}";
            }
        }

        public bool HasWarn()
        {
            return HasNotManager() || HasRepeatedManager();
        }

        public bool HasNotManager()
        {
            return Node.GetComponent<UITextManager>() == null;
        }

        public bool HasRepeatedManager()
        {
            return Node.GetComponents<UITextManager>().Length > 1;
        }

        public bool HasNullError()
        {
            if (Node.TryGetComponent(out UITextManager mgr))
            {
                return mgr.FontAssetPreset == null;
            }

            var text = Node.GetComponent<TextMeshProUGUI>();
            return text.font == null || text.fontSharedMaterial == null;
        }
    }

    public class UiTextAssetsEditorPrefabAssetNodeItemDrawer : OdinValueDrawer<UiTextPrefabAssetNode>
    {
        private static readonly float IconWidth = EditorGUIUtility.singleLineHeight;
        private static readonly float IconInterval = 2f;

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var value = ValueEntry.SmartValue;

            SirenixEditorGUI.BeginBox();
            SirenixEditorGUI.BeginBoxHeader();

            var headerRect = EditorGUILayout.GetControlRect(false);

            var label2 = GUIHelper.TempContent("      " + value.Label);

            var buttonRect = new Rect(headerRect)
            {
                x = headerRect.x + 17,
                width = IconWidth,
                height = IconWidth
            };

            var iconRect = new Rect(headerRect)
            {
                x = headerRect.xMax - IconWidth,
                width = IconWidth,
                height = IconWidth
            };

            if (value.HasWarn())
            {
                GUI.DrawTexture(iconRect, EditorIcons.UnityWarningIcon);
                iconRect.x -= IconWidth + IconInterval;
            }

            if (!value.Node.gameObject.activeSelf)
            {
                SdfIcons.DrawIcon(iconRect, SdfIconType.Lightbulb);
            }
            else if (!value.Node.gameObject.activeInHierarchy)
            {
                SdfIcons.DrawIcon(iconRect, SdfIconType.LightbulbOff);
            }

            value.Checked = EditorGUI.Toggle(buttonRect, value.Checked);

            Property.State.Expanded = SirenixEditorGUI.Foldout(headerRect, Property.State.Expanded, label2);
            SirenixEditorGUI.EndBoxHeader();

            if (SirenixEditorGUI.BeginFadeGroup(this, Property.State.Expanded))
            {
                foreach (var child in ValueEntry.Property.Children)
                {
                    child.Draw();
                }
            }

            SirenixEditorGUI.EndFadeGroup();
            SirenixEditorGUI.EndBox();
        }
    }
}
