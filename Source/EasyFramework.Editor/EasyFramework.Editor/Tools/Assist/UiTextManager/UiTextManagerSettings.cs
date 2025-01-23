using EasyFramework;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEditor;

namespace EasyFramework.Editor
{
    [EditorConfigAssetPath]
    public class UiTextManagerSettings : ScriptableObjectSingleton<UiTextManagerSettings>
    {
        [TitleGroupCN("配置")]
        [LabelText("管理位置")]
        public UiTextManagerWindow.ManagerPositions ManagerPosition = UiTextManagerWindow.ManagerPositions.InProject;

        [TitleGroupCN("配置")]
        [FolderPath(ParentFolder = "Assets")]
        [ShowIf("ManagerPosition", UiTextManagerWindow.ManagerPositions.InProject)]
        [LabelText("管理路径")]
        public string AssetsManagerPath = "Resources";

        [EnumToggleButtons]
        [TitleGroup("配置/显示模式", boldTitle: false)]
        [HideLabel]
        public UiTextManagerWindow.ViewModes ViewModes = UiTextManagerWindow.ViewModes.All;

        [TitleGroupCN("配置")]
        [InfoBoxCN("注意：资产视图中的预制体不会实时更新，可能会出现类似错误都解决了但是图标依然是错误，或者预制体删除了但依然在显示，需要再次“重新加载资源视图”",
            InfoMessageType.Warning)]
        [InfoBoxCN("任何资产配置的更改, 都需要点击“重新加载资源视图”才会实际应用")]
        [Button("重新加载资源视图")]
        [EnableIf("IsWindowOpen")]
        [UsedImplicitly]
        private void Rebuild()
        {
            UiTextManagerWindow.Instance.Rebuild();
        }

        [TitleGroupCN("视图配置")]
        [LabelText("自动打开选中预制体")]
        public bool AutoOpenSelectionPrefab = true;

        private bool IsWindowOpen => EditorWindow.HasOpenInstances<UiTextManagerWindow>();
    }
}
