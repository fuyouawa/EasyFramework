using EasyToolKit.Inspector;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.TileWorldPro;
using UnityEditor;

namespace EasyToolKit.TileWorldPro.Editor
{
    [CustomEditor(typeof(ChunkTerrainObject))]
    public class TerrainObjectEditor : EasyEditor
    {
        protected override void DrawTree()
        {
            Tree.BeginDraw();
            EditorGUILayout.LabelField("地形GUID", ((ChunkTerrainObject)target).TerrainDefinition.Guid.ToString("D"));
            Tree.DrawProperties();
            Tree.EndDraw();
        }
    }
}
