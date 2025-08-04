using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.Tilemap.Editor
{
    public interface IDrawingTool
    {
        void OnSceneGUI(TilemapCreator target, Vector3 hitPoint, Vector3? hitBlockPosition);
    }
}