using System;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Tilemap.Editor
{
    public class TilemapEditorIcons : Singleton<TilemapEditorIcons>
    {
        private TilemapEditorIcons()
        {
        }

        private Texture2D[,] _terrainTypeIconsAtlas;

        public Texture2D TerrainFillTypeIcon => GetTerrainTypeIcon(TerrainType.Fill);
        public Texture2D TerrainExteriorCornerTypeIcon => GetTerrainTypeIcon(TerrainType.TopLeftExteriorCorner);
        public Texture2D TerrainEdgeTypeIcon => GetTerrainTypeIcon(TerrainType.TopEdge);
        public Texture2D TerrainInteriorCornerTypeIcon => GetTerrainTypeIcon(TerrainType.TopLeftInteriorCorner);

        public Texture2D GetTerrainTypeIcon(TerrainType terrainType)
        {
            switch (terrainType)
            {
                case TerrainType.BottomLeftInteriorCorner:
                    return _terrainTypeIconsAtlas[0, 0];
                case TerrainType.BottomRightInteriorCorner:
                    return _terrainTypeIconsAtlas[0, 1];
                case TerrainType.TopRightInteriorCorner:
                    return _terrainTypeIconsAtlas[0, 2];
                case TerrainType.TopLeftInteriorCorner:
                    return _terrainTypeIconsAtlas[0, 3];
                case TerrainType.BottomRightExteriorCorner:
                    return _terrainTypeIconsAtlas[1, 0];
                case TerrainType.BottomLeftExteriorCorner:
                    return _terrainTypeIconsAtlas[1, 1];
                case TerrainType.TopRightExteriorCorner:
                    return _terrainTypeIconsAtlas[1, 2];
                case TerrainType.TopLeftExteriorCorner:
                    return _terrainTypeIconsAtlas[1, 3];
                case TerrainType.RightEdge:
                    return _terrainTypeIconsAtlas[2, 0];
                case TerrainType.BottomEdge:
                    return _terrainTypeIconsAtlas[2, 1];
                case TerrainType.LeftEdge:
                    return _terrainTypeIconsAtlas[2, 2];
                case TerrainType.TopEdge:
                    return _terrainTypeIconsAtlas[2, 3];
                case TerrainType.Fill:
                    return _terrainTypeIconsAtlas[3, 0];
                default:
                    throw new ArgumentOutOfRangeException(nameof(terrainType), terrainType, null);
            }
        }

        protected override void OnSingletonInit()
        {
            var directory = EditorAssetPaths.GetModuleAssetDirectory("Tilemap");
            var atlas = AssetDatabase.LoadAssetAtPath<Texture2D>(directory + "/TerrainTypeIconAtlas.png");
            _terrainTypeIconsAtlas = atlas.SliceByCount(4, 4);
        }
    }
}
