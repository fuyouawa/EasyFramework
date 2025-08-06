using EasyToolKit.Core;
using EasyToolKit.Core.Editor.Internal;
using System;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public class TileWorldIcons : Singleton<TileWorldIcons>
    {
        private Texture2D _terrainTypeIconsAtlas;
        private Texture2D[,] _terrainTypeIcons;

        private Texture2D _drawIconAtlas;
        private Texture2D[,] _drawIcons;

        private Texture2D _miscAtlas;
        private Texture2D[,] _miscIcons;

        private TileWorldIcons()
        {
        }

        private Texture2D[,] TerrainTypeIcons
        {
            get
            {
                if (_terrainTypeIconsAtlas == null)
                {
                    var directory = EditorAssetPaths.GetModuleEditorDirectory("TileWorldPro");
                    _terrainTypeIconsAtlas =
                        AssetDatabase.LoadAssetAtPath<Texture2D>(directory + "/TerrainTypeIconAtlas.png");
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
                    var directory = EditorAssetPaths.GetModuleEditorDirectory("TileWorldPro");
                    _drawIconAtlas = AssetDatabase.LoadAssetAtPath<Texture2D>(directory + "/DrawIconAtlas.png");
                }

                if (_drawIcons == null || _drawIcons.Length == 0 || _drawIcons[0, 0] == null)
                {
                    _drawIcons = _drawIconAtlas.SliceByCount(1, 4);
                }

                return _drawIcons;
            }
        }

        private Texture2D[,] MiscIcons
        {
            get
            {
                if (_miscAtlas == null)
                {
                    var directory = EditorAssetPaths.GetModuleEditorDirectory("TileWorldPro");
                    _miscAtlas = AssetDatabase.LoadAssetAtPath<Texture2D>(directory + "/MiscIconAtlas.png");
                }

                if (_miscIcons == null || _miscIcons.Length == 0 || _miscIcons[0, 0] == null)
                {
                    _miscIcons = _miscAtlas.SliceByCount(1, 4);
                }

                return _miscIcons;
            }
        }

        public Texture2D TerrainFillTypeIcon =>
            GetTerrainTileTypeIcon(TerrainTileRuleType.Fill);

        public Texture2D TerrainExteriorCornerTypeIcon =>
            GetTerrainTileTypeIcon(TerrainTileRuleType.TopLeftExteriorCorner);

        public Texture2D TerrainEdgeTypeIcon =>
            GetTerrainTileTypeIcon(TerrainTileRuleType.TopEdge);

        public Texture2D TerrainInteriorCornerTypeIcon =>
            GetTerrainTileTypeIcon(TerrainTileRuleType.TopLeftInteriorCorner);

        public Texture2D GetTerrainTileTypeIcon(TerrainTileRuleType ruleType)
        {
            switch (ruleType)
            {
                case TerrainTileRuleType.BottomLeftInteriorCorner:
                    return TerrainTypeIcons[0, 0];
                case TerrainTileRuleType.BottomRightInteriorCorner:
                    return TerrainTypeIcons[0, 1];
                case TerrainTileRuleType.TopRightInteriorCorner:
                    return TerrainTypeIcons[0, 2];
                case TerrainTileRuleType.TopLeftInteriorCorner:
                    return TerrainTypeIcons[0, 3];
                case TerrainTileRuleType.BottomRightExteriorCorner:
                    return TerrainTypeIcons[1, 0];
                case TerrainTileRuleType.BottomLeftExteriorCorner:
                    return TerrainTypeIcons[1, 1];
                case TerrainTileRuleType.TopRightExteriorCorner:
                    return TerrainTypeIcons[1, 2];
                case TerrainTileRuleType.TopLeftExteriorCorner:
                    return TerrainTypeIcons[1, 3];
                case TerrainTileRuleType.RightEdge:
                    return TerrainTypeIcons[2, 0];
                case TerrainTileRuleType.BottomEdge:
                    return TerrainTypeIcons[2, 1];
                case TerrainTileRuleType.LeftEdge:
                    return TerrainTypeIcons[2, 2];
                case TerrainTileRuleType.TopEdge:
                    return TerrainTypeIcons[2, 3];
                case TerrainTileRuleType.Fill:
                    return TerrainTypeIcons[3, 0];
                default:
                    throw new ArgumentOutOfRangeException(nameof(ruleType), ruleType, null);
            }
        }

        public Texture2D GetTerrainRuleGridTypeIcon(TerrainRuleGridType gridType)
        {
            switch (gridType)
            {
                case TerrainRuleGridType.Empty:
                    return MiscIcons[0, 1];
                case TerrainRuleGridType.Contain:
                    return MiscIcons[0, 0];
                case TerrainRuleGridType.Option:
                    return MiscIcons[0, 2];
                case TerrainRuleGridType.Union:
                    return MiscIcons[0, 3];
                default:
                    throw new ArgumentOutOfRangeException(nameof(gridType), gridType, null);
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
                case DrawMode.LineBrush:
                    return DrawIcons[0, 2];
                case DrawMode.RectangleBrush:
                    return DrawIcons[0, 3];
                default:
                    throw new ArgumentOutOfRangeException(nameof(drawMode), drawMode, null);
            }
        }
    }
}
