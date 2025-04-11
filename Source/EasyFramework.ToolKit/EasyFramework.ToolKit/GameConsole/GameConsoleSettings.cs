using UnityEngine;

namespace EasyFramework.ToolKit
{
    [SettingsAssetPath, CreateAssetMenu(fileName = nameof(GameConsoleSettings), menuName = "EasyFramework/Create GameConsoleSettings")]
    public class GameConsoleSettings : ScriptableObjectSingleton<GameConsoleSettings>
    {
        public Sprite InfoLogIcon;
        public Sprite WarnLogIcon;
        public Sprite ErrorLogIcon;

        public GameConsoleLogItem LogItemPrefab;
    }
}
