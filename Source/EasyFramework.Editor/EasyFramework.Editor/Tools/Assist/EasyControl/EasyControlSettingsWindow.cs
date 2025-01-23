using JetBrains.Annotations;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace EasyFramework.Editor
{
    public class EasyControlSettingsWindow : OdinEditorWindow
    {
        [MenuItem("Tools/EasyGameFramework/Settings/Easy Control Settings")]
        [UsedImplicitly]
        public static void ShowWindow()
        {
            GetWindow<EasyControlSettingsWindow>("Easy Control Settings");
        }

        protected override object GetTarget()
        {
            return EasyControlSettings.Instance;
        }
    }
}
