using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Pokemon.UI;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EasyGameFramework.Editor
{
    public class UiTextPrefabAsset
    {
        [TitleGroupCN("基础信息")]
        [ReadOnly]
        [LabelText("资产路径")]
        public string AssetPath;

        public GameObject Asset => AssetDatabase.LoadAssetAtPath<GameObject>(AssetPath);

        public UiTextPrefabAsset(string assetPath)
        {
            AssetPath = assetPath;
        }

        [Flags]
        public enum ItemsViewModes
        {
            [LabelText("正确的项")]
            Correct = 1,

            [LabelText("报错的项")]
            Error = 1 << 1,

            [LabelText("警告的项")]
            Warn = 1 << 2,

            [LabelText("未激活的项")]
            Disable = 1 << 3,

            [LabelText("所有项")]
            All = Correct | Error | Warn | Disable,
        }

        [EnumToggleButtons]
        [TitleGroupCN("组件列表显示模式")]
        [HideLabel]
        public ItemsViewModes ViewModes = ItemsViewModes.All;

        [TitleGroupCN("组件列表显示模式/列表项勾选操作")]
        [HorizontalGroup("组件列表显示模式/列表项勾选操作/布局")]
        [Button("勾选列表中所有项", Icon = SdfIconType.CheckSquare)]
        public void CheckAll()
        {
            foreach (var item in TextItems)
            {
                item.Checked = true;
            }
        }

        [TitleGroupCN("组件列表显示模式/列表项勾选操作")]
        [HorizontalGroup("组件列表显示模式/列表项勾选操作/布局")]
        [Button("取消勾选列表中所有项", Icon = SdfIconType.XSquare)]
        public void CheckNone()
        {
            foreach (var item in TextItems)
            {
                item.Checked = false;
            }
        }

        [TitleGroupCN("组件列表显示模式/预制体操作")]
        [HorizontalGroup("组件列表显示模式/预制体操作/布局")]
        [Button("打开预制体", Icon = SdfIconType.Folder2Open)]
        public void OpenPrefab()
        {
            PrefabStageUtility.OpenPrefab(AssetPath);
        }

        [TitleGroupCN("组件列表显示模式/预制体操作")]
        [HorizontalGroup("组件列表显示模式/预制体操作/布局")]
        [Button("在Hierarchy中选中勾选的实例", Icon = SdfIconType.ListCheck)]
        private void Select()
        {
            var ps = PrefabStageUtility.OpenPrefab(AssetPath);
            Selection.objects = TextItems
                .Where(i => i.Checked)
                .Select(i => ps.prefabContentsRoot.transform.Find(i.FullName).gameObject as Object)
                .ToArray();
        }

        [TitleGroupCN("组件列表显示模式/预制体分析")]
        [FoldoutGroup("组件列表显示模式/预制体分析/自动分析处理的条件")]
        [InfoBoxCN("如果没有UITextManager组件会自动添加")]
        [LabelText("条件1")]
        public bool Condition1 = true;

        [TitleGroupCN("组件列表显示模式/预制体分析")]
        [FoldoutGroup("组件列表显示模式/预制体分析/自动分析处理的条件")]
        [InfoBoxCN("如果UITextManager的\"字体资产预设\"依然null，根据UI文本组件的字体和材质设置，判断属于\"预设管理器\"中的哪个字体资产预设，然后自动赋值")]
        [LabelText("条件2")]
        public bool Condition2 = true;

        [TitleGroupCN("组件列表显示模式/预制体分析")]
        [FoldoutGroup("组件列表显示模式/预制体分析/自动分析处理的条件")]
        [InfoBoxCN("如果UITextManager的\"字体资产预设\"依然是null，会使用\"预设管理器\"中定义的\"默认字体资产预设\"")]
        [LabelText("条件3")]
        public bool Condition3 = true;

        [TitleGroupCN("组件列表显示模式/预制体分析")]
        [FoldoutGroup("组件列表显示模式/预制体分析/自动分析处理的条件")]
        [InfoBoxCN("如果组件上有多个重复的UITextManager，则只保留第一个，删除其他重复项")]
        [LabelText("条件4")]
        public bool Condition4 = true;

        [TitleGroupCN("组件列表显示模式/预制体分析")]
        [Button("自动分析处理警告/错误", Icon = SdfIconType.Tools)]
        [UsedImplicitly]
        private void AutoHandleWarn()
        {
            var msg = "您确定要使用自动处理吗";
            if (EditorUtility.DisplayDialog("警告", msg, "确认", "取消"))
            {
                OpenPrefab();
                foreach (var item in TextItems)
                {
                    var mgr = item.Node.GetComponent<UITextManager>();
                    if (mgr == null)
                    {
                        // 如果没有UITextManager组件会自动添加
                        if (Condition1)
                        {
                            mgr = item.Node.gameObject.AddComponent<UITextManager>();
                        }
                        else
                        {
                            continue;
                        }
                    }
                    var text = item.Node.GetComponent<TextMeshProUGUI>();
                    // 根据UI文本组件的字体和材质设置, 判断属于"预设管理器"中的哪个字体资产预设, 然后自动赋值
                    if (Condition2 && mgr.FontAssetPreset == null)
                    {
                        for (int i = 0; i < UITextPresetsManager.Instance.FontAssetPresets.Count; i++)
                        {
                            var preset = UITextPresetsManager.Instance.FontAssetPresets[i];
                            if (preset.FontAsset == text.font && preset.Material == text.fontSharedMaterial)
                            {
                                mgr.FontAssetPresetIndex = i;
                            }
                        }
                    }

                    // 如果UITextManager的"字体资产预设"依然是null, 会使用"预设管理器"中定义的"默认字体资产预设"
                    if (Condition3 && mgr.FontAssetPreset == null)
                    {
                        mgr.FontAssetPresetIndex = UITextPresetsManager.Instance.DefaultFontAssetPresetIndex;
                    }

                    if (Condition4)
                    {
                        var mgrs = item.Node.GetComponents<UITextManager>();
                        if (mgrs.Length > 1)
                        {
                            foreach (var m in mgrs.Skip(1))
                            {
                                GameObject.DestroyImmediate(m);
                            }
                        }
                        Debug.Assert(item.Node.GetComponents<UITextManager>().Length <= 1);
                    }
                }
            }
        }
        
        public void Analyse()
        {
            ClearCache();
            foreach (var text in Asset.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                var item = new UiTextPrefabAssetNode(text.gameObject.transform.GetRelativePath(Asset.transform, false), AssetPath);
                if (TextItemFilter(item))
                {
                    TextItems.Add(item);
                }
            }
        }

        private bool TextItemFilter(UiTextPrefabAssetNode item)
        {
            if (ViewModes == 0)
                return false;
            if (ViewModes == ItemsViewModes.All)
                return true;

            var warn = item.HasWarn();

            if (warn && ViewModes.HasFlag(ItemsViewModes.Warn))
            {
                return true;
            }

            if (!item.Node.gameObject.activeInHierarchy && ViewModes.HasFlag(ItemsViewModes.Disable))
            {
                return true;
            }

            if (!warn && ViewModes.HasFlag(ItemsViewModes.Correct))
            {
                return true;
            }

            return false;
        }

        [TitleCN("UI文本组件实例视图")]
        [ListDrawerSettings(IsReadOnly = true, OnTitleBarGUI = "OnTextItemsTitleBarGUI")]
        [LabelText("实例列表")]
        public List<UiTextPrefabAssetNode> TextItems = new();

        public void ClearCache()
        {
            TextItems.Clear();
        }

        public bool HasWarn()
        {
            foreach (var item in TextItems)
            {
                if (item.HasWarn())
                {
                    return true;
                }
            }

            return false;
        }
        
        [UsedImplicitly]
        private void OnTextItemsTitleBarGUI()
        {
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
            {
                Analyse();
            }

            if (SirenixEditorGUI.ToolbarButton(SdfIconType.Question))
            {
                //TODO 提示
                EditorUtility.DisplayDialog("提示", "1.如果有关于UI文本组件实例列表的更改, 需要点一下旁边的重置按钮重新分析\n" +
                                                  "2.还没想好", "确认");
            }
        }
    }
}
