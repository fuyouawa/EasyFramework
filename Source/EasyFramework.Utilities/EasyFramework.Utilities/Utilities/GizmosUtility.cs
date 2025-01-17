using UnityEngine;

namespace EasyFramework.Utilities
{
    public static class GizmosUtility
    {
        public static void DrawMeshRect(Vector2 center, Vector2 size, Vector2 density)
        {
            var halfSize = size / 2;

            var p1 = new Vector2(center.x - halfSize.x, center.y - halfSize.y);
            var p2 = new Vector2(center.x + halfSize.x, center.y - halfSize.y);
            var p3 = new Vector2(center.x + halfSize.x, center.y + halfSize.y);
            var p4 = new Vector2(center.x - halfSize.x, center.y + halfSize.y);

            if (density.x > 0.01f)
            {
                Gizmos.DrawLine(p2, p3);
                Gizmos.DrawLine(p4, p1);
                int gridX = Mathf.FloorToInt(size.x / density.x);
                var stepX = size.x / gridX;
                for (int i = 1; i < gridX; i++)
                {
                    var x = p1.x + i * stepX;
                    Gizmos.DrawLine(new Vector3(x, p1.y, 0), new Vector3(x, p4.y, 0));
                }
            }

            if (density.y > 0.01f)
            {
                Gizmos.DrawLine(p1, p2);
                Gizmos.DrawLine(p3, p4);
                int gridY = Mathf.FloorToInt(size.y / density.y);

                var stepY = size.y / gridY;
                for (int i = 1; i < gridY; i++)
                {
                    var y = p1.y + i * stepY;
                    Gizmos.DrawLine(new Vector3(p1.x, y, 0), new Vector3(p2.x, y, 0));
                }
            }
        }
        
        public static void DrawCube(Transform transform, Vector3 offset, Vector3 cubeSize, bool wireOnly)
        {
            Matrix4x4 rotationMatrix = transform.localToWorldMatrix;
            Gizmos.matrix = rotationMatrix;
            if (wireOnly)
            {
                Gizmos.DrawWireCube(offset, cubeSize);
            }
            else
            {
                Gizmos.DrawCube(offset, cubeSize);
            }
        }
        
        public static void DrawMeshRect(Vector2 center, Vector2 size)
        {
            DrawMeshRect(center, size, new Vector2(0.1f, 0.1f));
        }
    }
}
