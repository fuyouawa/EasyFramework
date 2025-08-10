using EasyToolKit.Inspector.Editor;
using EasyToolKit.TileWorldPro;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public class ChunkAreaDrawer : EasyValueDrawer<ChunkArea>
    {
        protected override void DrawProperty(GUIContent label)
        {
            var value = ValueEntry.SmartValue;

            EditorGUI.BeginChangeCheck();
            var position = EditorGUILayout.Vector2IntField("Position", value.Position);
            var size = EditorGUILayout.Vector2IntField("Size", value.Size);
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = new ChunkArea(position, size);
            }
        }
    }
}