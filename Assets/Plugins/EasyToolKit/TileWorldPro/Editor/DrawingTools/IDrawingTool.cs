using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public interface IDrawingTool
    {
        void OnSceneGUI(TileWorldDesigner target, Vector3 hitPoint, Vector3? hitTileWorldPosition);
    }
}