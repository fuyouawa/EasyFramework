using System;
using Sirenix.OdinInspector;
using EasyGameFramework;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyGameFramework.Editor
{
    [Flags]
    public enum PrefabViewModes
    {
        [LabelText("正确的预制体")]
        Correct = 1,
        [LabelText("有警告的预制体")]
        Warn = 1 << 2,
        [LabelText("所有预制体")]
        All = Correct | Warn,
    }

    public class UiTextAssetsEditorSettings
    {
        [TitleGroupCN("预制体资产配置")]
        [FolderPath(ParentFolder = "Assets")]
        [LabelText("管理路径")]
        public string AssetsManagerPath = "Resources";
        [EnumToggleButtons]
        [TitleGroupCN("预制体资产配置/显示模式", boldTitle:false)]
        [HideLabel]
        public PrefabViewModes ViewModes = PrefabViewModes.All;
        
        [TitleGroupCN("预制体资产配置")]
        [InfoBoxCN("注意：资产视图中的预制体不会实时更新，可能会出现类似错误都解决了但是图标依然是错误，或者预制体删除了但依然在显示，需要再次“重新加载资源视图”", InfoMessageType.Warning)]
        [InfoBoxCN("任何资产配置的更改, 都需要点击\"重新加载资源视图\"才会实际应用")]
        [Button("重新加载资源视图")]
        [UsedImplicitly]
        private void Rebuild()
        {
            UiTextAssetsManagerWindow.Instance.Rebuild();
        }


        [TitleGroupCN("视图配置")]
        [LabelText("自动打开选中预制体")]
        public bool AutoOpenSelectionPrefab = true;

        [TitleGroup("数据存储")]
        [InfoBoxCN("注意: 当编辑器重新编译的时候, 未保存的更改可能会丢失", InfoMessageType.Warning)]
        [LabelText("自动保存间隔(秒)")]
        public float AutoSaveInterval = 5f;

        [TitleGroup("数据存储")]
        [Button("保存编辑器设置")]
        [UsedImplicitly]
        private void Save()
        {
            UiTextAssetsManagerWindow.Instance.Save();
            AssetDatabase.Refresh();
        }
        
        [TitleGroup("数据存储")]
        [Button("加载编辑器设置")]
        [UsedImplicitly]
        private void Load()
        {
            UiTextAssetsManagerWindow.Instance.Load();
        }
    }
}
