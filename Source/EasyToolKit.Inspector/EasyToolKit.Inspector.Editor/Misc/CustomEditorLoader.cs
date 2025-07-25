using UnityEditor.Callbacks;

namespace EasyToolKit.Inspector.Editor
{
    internal static class CustomEditorLoader
    {
        [DidReloadScripts]
        static CustomEditorLoader()
        {
            InspectorConfigAsset.Instance.UpdateEditors();
        }
    }
}
