using System;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Tilemap.Editor
{
    public static class TilemapHandles
    {
        private static readonly float Epsilon = TilemapUtility.Epsilon;

        public static void DrawBase(Vector3 startPosition, Vector2 range, float tileSize, Color color)
        {
            EasyHandleHelper.PushZTest(UnityEngine.Rendering.CompareFunction.LessEqual);
            EasyHandleHelper.PushColor(color);
            for (int x = 0; x <= range.x; x++)
            {
                var start = startPosition + Vector3.right * (x * tileSize);
                var end = start + Vector3.forward * (tileSize * range.y);
                Handles.DrawLine(start, end);
            }

            for (int y = 0; y <= range.y; y++)
            {
                var start = startPosition + Vector3.forward * (y * tileSize);
                var end = start + Vector3.right * (tileSize * range.x);
                Handles.DrawLine(start, end);
            }

            EasyHandleHelper.PopColor();
            EasyHandleHelper.PopZTest();
        }

        public static void DrawTileCubes(Vector3 startPosition, TerrainMap map, float tileSize)
        {
            foreach (var position in map)
            {
                var blockPosition = TilemapUtility.TilePositionToWorldPosition(startPosition, position.TilePosition, tileSize);

                if (position.Definition.DrawDebugCube)
                {
                    if (position.Definition.EnableZTestForDebugCube)
                    {
                        EasyHandleHelper.PushZTest(UnityEngine.Rendering.CompareFunction.LessEqual);
                    }

                    DrawCube(blockPosition, tileSize, position.Definition.DebugCubeColor);

                    if (position.Definition.EnableZTestForDebugCube)
                    {
                        EasyHandleHelper.PopZTest();
                    }
                }
            }
        }

        public static void DrawDebugHitPointGUI(Vector3 hitPoint)
        {
            Handles.BeginGUI();

            Vector2 guiPosition = HandleUtility.WorldToGUIPoint(hitPoint);
            GUI.Label(new Rect(guiPosition.x + 10, guiPosition.y - 10, 200, 20), $"{hitPoint}");

            Handles.EndGUI();
        }

        public static void DrawDebugRuleTypeGUI(Vector3 tileWorldPosition, TerrainTileRuleType ruleType)
        {
            var ruleTypeText = ruleType switch
            {
                TerrainTileRuleType.Fill => "填充",
                TerrainTileRuleType.TopEdge => "顶边缘",
                TerrainTileRuleType.LeftEdge => "左边缘",
                TerrainTileRuleType.BottomEdge => "底边缘",
                TerrainTileRuleType.RightEdge => "右边缘",
                TerrainTileRuleType.TopLeftExteriorCorner => "左上外角",
                TerrainTileRuleType.TopRightExteriorCorner => "右上外角",
                TerrainTileRuleType.BottomRightExteriorCorner => "右下外角",
                TerrainTileRuleType.BottomLeftExteriorCorner => "左下外角",
                TerrainTileRuleType.TopLeftInteriorCorner => "左上内角",
                TerrainTileRuleType.TopRightInteriorCorner => "右上内角",
                TerrainTileRuleType.BottomRightInteriorCorner => "右下内角",
                TerrainTileRuleType.BottomLeftInteriorCorner => "左下内角",
                _ => throw new NotImplementedException(),
            };

            Handles.BeginGUI();

            Vector2 guiPosition = HandleUtility.WorldToGUIPoint(tileWorldPosition);
            GUI.Label(new Rect(guiPosition.x, guiPosition.y - 20, 200, 20), $"{ruleTypeText}");

            Handles.EndGUI();
        }

        public static void DrawDebugBlockGUI(Vector3 gridWorldPosition, float distance)
        {
            Handles.BeginGUI();

            Vector2 guiPosition = HandleUtility.WorldToGUIPoint(gridWorldPosition);
            GUI.Label(new Rect(guiPosition.x, guiPosition.y - 20, 200, 20), $"{gridWorldPosition} - {distance:F2}");

            Handles.EndGUI();
        }

        public static void DrawHitCube(Vector3 blockPosition, float tileSize, Color hitColor, Color? surroundingColor = null)
        {
            DrawCube(blockPosition, tileSize, hitColor);
            DrawFillCube(blockPosition, tileSize, hitColor.MulA(0.7f));

            if (surroundingColor != null)
            {
                DrawSquare(blockPosition + Vector3.forward, tileSize, surroundingColor.Value);
                DrawSquare(blockPosition + Vector3.back, tileSize, surroundingColor.Value);
                DrawSquare(blockPosition + Vector3.left, tileSize, surroundingColor.Value);
                DrawSquare(blockPosition + Vector3.right, tileSize, surroundingColor.Value);

                DrawSquare(blockPosition + Vector3.forward + Vector3.left, tileSize, surroundingColor.Value.MulA(0.5f));
                DrawSquare(blockPosition + Vector3.forward + Vector3.right, tileSize, surroundingColor.Value.MulA(0.5f));
                DrawSquare(blockPosition + Vector3.back + Vector3.left, tileSize, surroundingColor.Value.MulA(0.5f));
                DrawSquare(blockPosition + Vector3.back + Vector3.right, tileSize, surroundingColor.Value.MulA(0.5f));
            }
        }

        public static void DrawSquare(Vector3 blockPosition, float tileSize, Color color)
        {
            var size = new Vector3(1f, 0.05f, 1f) * tileSize;
            var center = blockPosition + 0.5f * tileSize * new Vector3(1f, 0f, 1f);
            EasyHandleHelper.PushColor(color);
            Handles.DrawWireCube(center, size);
            EasyHandleHelper.PopColor();
        }

        public static void DrawCube(Vector3 blockPosition, float tileSize, Color color)
        {
            EasyHandleHelper.PushColor(color);

            var center = blockPosition + Vector3.one * (tileSize * 0.5f);
            Handles.DrawWireCube(center, Vector3.one * tileSize);

            EasyHandleHelper.PopColor();
        }

        public static void DrawFillCube(Vector3 blockPosition, float tileSize, Color color)
        {
            EasyHandleHelper.PushColor(color);

            var center = blockPosition + Vector3.one * (tileSize * 0.5f);
            Handles.CubeHandleCap(0, center, Quaternion.identity, tileSize, EventType.Repaint);

            EasyHandleHelper.PopColor();
        }
    }
}
