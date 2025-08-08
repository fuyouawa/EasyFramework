using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public struct DrawingToolContext
    {
        public TileWorldDesigner Target;
        public Vector3 HitPoint;
        public Vector3 HitTileWorldPosition;
        public TerrainDefinition TerrainDefinition;
    }

    public interface IDrawingTool
    {
        void OnSceneGUI(DrawingToolContext context);
    }
}