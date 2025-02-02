using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor.Drawer
{
    //TODO SerializedNullableDrawer
    public class SerializedNullableDrawer<T> : OdinValueDrawer<SerializedNullable<T>>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            EasyEditorGUI.MessageBox("TODO", MessageType.Warning);
        }
    }
}
