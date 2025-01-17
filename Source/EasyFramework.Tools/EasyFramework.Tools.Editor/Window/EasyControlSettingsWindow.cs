using JetBrains.Annotations;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace EasyGameFramework.Editor
{
    public class EasyControlSettingsWindow : OdinEditorWindow
    {
        [MenuItem("Tools/EasyGameFramework/Settings/EasyControlSettings")]
        [UsedImplicitly]
        public static void ShowWindow()
        {
            GetWindow<EasyControlSettingsWindow>("EasyControlSettings");
        }

        protected override object GetTarget()
        {
            return EasyControlSettings.Instance;
        }
    }
}
