using EasyToolKit.Inspector;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.Tilemap;
using UnityEditor;

namespace EasyToolKit.Tilemap.Editor
{
    [CustomEditor(typeof(TerrainObject))]
    public class TerrainObjectEditor : EasyEditor
    {
        protected override void DrawTree()
        {
            Tree.BeginDraw();

            EditorGUILayout.LabelField("地形瓦片定义GUID", ((TerrainObject)target).TargetTerrainTileDefinitionGuid.ToString("D"));

            Tree.EndDraw();
        }
    }
}