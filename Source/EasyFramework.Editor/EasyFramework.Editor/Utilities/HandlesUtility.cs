using System;
using EasyFramework.Core;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor
{
    public enum TriangleOrientations
    {
        Up,
        Down,
        Left,
        Right
    }

    public static class HandlesUtility
    {
        public static Rect GetWorldUnitsRectWithScaleFactor(Vector3 worldPosition, float sizeInWorldUnits)
        {
            // 计算场景视图中的缩放比例
            float scaleFactor = HandleUtility.GetHandleSize(worldPosition);

            // 将世界坐标转换为屏幕坐标
            Vector3 screenPosition = HandleUtility.WorldToGUIPoint(worldPosition);

            // 根据缩放比例调整纹理绘制的大小
            float size = sizeInWorldUnits / scaleFactor * 100f; // 乘以100是为了换算为屏幕像素

            // 创建一个矩形来确定纹理绘制的位置和大小
            return new Rect(screenPosition.x - size / 2, screenPosition.y - size / 2, size, size);
        }

        public static void DrawTextureWithScaleFactor(Vector3 worldPosition, Texture image, float sizeInWorldUnits)
        {
            var rect = GetWorldUnitsRectWithScaleFactor(worldPosition, sizeInWorldUnits);

            Handles.BeginGUI();
            GUI.DrawTexture(rect, image);
            Handles.EndGUI();
        }

        public static void DrawRectangle(Rect rectangle)
        {
            Handles.DrawSolidRectangleWithOutline(rectangle, Color.white, Color.white);
        }

        public static void DrawRectangle2D(Vector2 position, Vector2 size)
        {
            DrawRectangle(position.ToVec3(), size.ToVec3());
        }

        public static void LabelWithScaleFactor(Vector3 worldPosition, string text, GUIStyle style)
        {
            // 获取场景视图相机的缩放比例
            float scaleFactor = HandleUtility.GetHandleSize(worldPosition);

            // 缩放文字大小
            var style2 = new GUIStyle(style)
            {
                fontSize = Mathf.CeilToInt(style.fontSize / scaleFactor) // 根据缩放比例调整字体大小
            };

            Handles.Label(worldPosition, text, style2);
        }

        public static void DrawLine2D(Vector2 p1, Vector2 p2)
        {
            Handles.DrawLine(p1.ToVec3(), p2.ToVec3());
        }

        public static void DrawRectangle(Vector3 position, Vector3 size)
        {
            Handles.DrawSolidRectangleWithOutline(new Rect(position, size), Color.white, Color.white);
        }

        public static void DrawTriangleInRect(Vector3 position, Vector3 size, TriangleOrientations orientation)
        {
            DrawTriangleInRect(new Rect(position, size), orientation);
        }

        public static void DrawTriangleInRect(Rect rect, TriangleOrientations orientation)
        {
            Vector3 v1;
            Vector3 v2;
            Vector3 v3;

            switch (orientation)
            {
                case TriangleOrientations.Up:
                    v1 = new Vector3(rect.xMin + rect.width / 2, rect.yMax);
                    v2 = new Vector3(rect.xMin, rect.yMin);
                    v3 = new Vector3(rect.xMax, rect.yMin);
                    break;
                case TriangleOrientations.Down:
                    v1 = new Vector3(rect.xMin + rect.width / 2, rect.yMin);
                    v2 = new Vector3(rect.xMin, rect.yMax);
                    v3 = new Vector3(rect.xMax, rect.yMax);
                    break;
                case TriangleOrientations.Left:
                    v1 = new Vector3(rect.xMin, rect.yMin + rect.height / 2);
                    v2 = new Vector3(rect.xMax, rect.yMin);
                    v3 = new Vector3(rect.xMax, rect.yMax);
                    break;
                case TriangleOrientations.Right:
                    v1 = new Vector3(rect.xMax, rect.yMin + rect.height / 2);
                    v2 = new Vector3(rect.xMin, rect.yMin);
                    v3 = new Vector3(rect.xMin, rect.yMax);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }

            Handles.DrawAAConvexPolygon(v1, v2, v3);
        }
    }
}
