using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Tilemap.Editor
{
    [CustomEditor(typeof(TilemapCreator))]
    public class TilemapCreatorEditor : EasyEditor
    {
        public TilemapCreator Target => (TilemapCreator)target;

        void OnSceneGUI()
        {
            if (Target.Asset == null)
                return;

            var baseRange = Target.Asset.Settings.BaseRange;
            var tileSize = Target.Asset.Settings.TileSize;

            EasyHandleHelper.PushColor(Target.Asset.Settings.BaseColor);
            for (int x = 0; x <= baseRange.x; x++)
            {
                var start = Target.transform.position + Vector3.right * x * tileSize;
                var end = start + Vector3.forward * tileSize * baseRange.y;
                Handles.DrawLine(start, end);
            }

            for (int y = 0; y <= baseRange.y; y++)
            {
                var start = Target.transform.position + Vector3.forward * y * tileSize;
                var end = start + Vector3.right * tileSize * baseRange.x;
                Handles.DrawLine(start, end);
            }
            EasyHandleHelper.PopColor();
        }
    }
}
