using EasyToolKit.Core;
using EasyToolKit.Core.Editor.Internal;
using System;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Tilemap.Editor
{
    public class TilemapEditorIcons : Singleton<TilemapEditorIcons>
    {

        private Texture2D _terrainTypeIconsAtlas;
        private Texture2D[,] _terrainTypeIcons;

        private Texture2D _drawIconAtlas;
        private Texture2D[,] _drawIcons;

        private TilemapEditorIcons()
        {
        }

        private Texture2D[,] TerrainTypeIcons
        {
            get
            {
                if (_terrainTypeIconsAtlas == null)
                {
                    var directory = EditorAssetPaths.GetModuleEditorDirectory("Tilemap");
                    _terrainTypeIconsAtlas = AssetDatabase.LoadAssetAtPath<Texture2D>(directory + "/TerrainTypeIconAtlas.png");
                }

                if (_terrainTypeIcons == null || _terrainTypeIcons.Length == 0 || _terrainTypeIcons[0, 0] == null)
                {
                    _terrainTypeIcons = _terrainTypeIconsAtlas.SliceByCount(4, 4);
                }

                return _terrainTypeIcons;
            }
        }

        private Texture2D[,] DrawIcons
        {
            get
            {
                if (_drawIconAtlas == null)
                {
                    var directory = EditorAssetPaths.GetModuleEditorDirectory("Tilemap");
                    _drawIconAtlas = AssetDatabase.LoadAssetAtPath<Texture2D>(directory + "/DrawIconAtlas.png");
                }
                if (_drawIcons == null || _drawIcons.Length == 0 || _drawIcons[0, 0] == null)
                {
                    _drawIcons = _drawIconAtlas.SliceByCount(1, 2);
                }
                return _drawIcons;
            }
        }

        public Texture2D TerrainFillTypeIcon => GetTerrainTypeIcon(TerrainRuleType.Fill);
        public Texture2D TerrainExteriorCornerTypeIcon => GetTerrainTypeIcon(TerrainRuleType.TopLeftExteriorCorner);
        public Texture2D TerrainEdgeTypeIcon => GetTerrainTypeIcon(TerrainRuleType.TopEdge);
        public Texture2D TerrainInteriorCornerTypeIcon => GetTerrainTypeIcon(TerrainRuleType.TopLeftInteriorCorner);

        public Texture2D GetTerrainTypeIcon(TerrainRuleType terrainRuleType)
        {
            switch (terrainRuleType)
            {
                case TerrainRuleType.BottomLeftInteriorCorner:
                    return TerrainTypeIcons[0, 0];
                case TerrainRuleType.BottomRightInteriorCorner:
                    return TerrainTypeIcons[0, 1];
                case TerrainRuleType.TopRightInteriorCorner:
                    return TerrainTypeIcons[0, 2];
                case TerrainRuleType.TopLeftInteriorCorner:
                    return TerrainTypeIcons[0, 3];
                case TerrainRuleType.BottomRightExteriorCorner:
                    return TerrainTypeIcons[1, 0];
                case TerrainRuleType.BottomLeftExteriorCorner:
                    return TerrainTypeIcons[1, 1];
                case TerrainRuleType.TopRightExteriorCorner:
                    return TerrainTypeIcons[1, 2];
                case TerrainRuleType.TopLeftExteriorCorner:
                    return TerrainTypeIcons[1, 3];
                case TerrainRuleType.RightEdge:
                    return TerrainTypeIcons[2, 0];
                case TerrainRuleType.BottomEdge:
                    return TerrainTypeIcons[2, 1];
                case TerrainRuleType.LeftEdge:
                    return TerrainTypeIcons[2, 2];
                case TerrainRuleType.TopEdge:
                    return TerrainTypeIcons[2, 3];
                case TerrainRuleType.Fill:
                    return TerrainTypeIcons[3, 0];
                default:
                    throw new ArgumentOutOfRangeException(nameof(terrainRuleType), terrainRuleType, null);
            }
        }

        public Texture2D GetDrawModeIcon(DrawMode drawMode)
        {
            switch (drawMode)
            {
                case DrawMode.Brush:
                    return DrawIcons[0, 0];
                case DrawMode.Eraser:
                    return DrawIcons[0, 1];
                default:
                    throw new ArgumentOutOfRangeException(nameof(drawMode), drawMode, null);
            }
        }
    }
}
