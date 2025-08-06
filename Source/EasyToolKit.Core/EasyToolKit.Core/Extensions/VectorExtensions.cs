using UnityEngine;

namespace EasyToolKit.Core
{
    public static class VectorExtensions
    {
        public static Vector2Int ToCrossDirection(this Vector2 v, float zeroThreshold = 0.01f)
        {
            v = v.normalized;

            var absX = Mathf.Abs(v.x);
            var absY = Mathf.Abs(v.y);

            if (absX <= zeroThreshold && absY <= zeroThreshold)
            {
                return Vector2Int.zero;
            }

            if (v.x > zeroThreshold)
            {
                if (v.y > zeroThreshold)
                {
                    return v.x > v.y ? Vector2Int.right : Vector2Int.up;
                }
                else
                {
                    return v.x > absY ? Vector2Int.right : Vector2Int.down;
                }
            }
            else
            {
                if (v.y > zeroThreshold)
                {
                    return absX > v.y ? Vector2Int.left : Vector2Int.up;
                }
                else
                {
                    return absX > absY ? Vector2Int.left : Vector2Int.down;
                }
            }
        }

        public static bool IsVertical(this Vector2 v)
        {
            var v1 = ToCrossDirection(v);
            return v1 == Vector2Int.up || v1 == Vector2Int.down;
        }

        public static Vector3 ToVector3(this Vector2 v, float z = 0f)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector2 ToVector2(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector2 Rotate(this Vector2 v, float angle)
        {
            float rad = angle * Mathf.Deg2Rad; // 将角度转换为弧度
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            // 计算旋转后的新向量
            float newX = v.x * cos - v.y * sin;
            float newY = v.x * sin + v.y * cos;

            return new Vector2(newX, newY);
        }


        public static Vector2 Round(this Vector2 v)
        {
            return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
        }


        public static Vector3 Round(this Vector3 v)
        {
            return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
        }

        public static Vector2 ToVector2(this Vector2Int v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector3 ToVector3(this Vector2Int v, int z = 0)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector3 ToVector3(this Vector3Int v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static Vector2 ToVector2(this Vector3Int v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector3Int ToVec3Int(this Vector2Int v)
        {
            return new Vector3Int(v.x, v.y);
        }

        public static Vector2Int ToVec2Int(this Vector3Int v)
        {
            return new Vector2Int(v.x, v.y);
        }

        public static Vector2Int RoundToVector2Int(this Vector2 v)
        {
            return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }

        public static Vector3Int RoundToVector3Int(this Vector2 v)
        {
            return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }

        public static Vector3Int RoundToVector3Int(this Vector3 v)
        {
            return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
        }

        public static Vector2Int RoundToVector2Int(this Vector3 v)
        {
            return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }

        public static Vector2Int FloorToVector2Int(this Vector2 v)
        {
            return new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
        }

        public static Vector3Int FloorToVector3Int(this Vector2 v)
        {
            return new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
        }

        public static Vector3Int FloorToVector3Int(this Vector3 v)
        {
            return new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));
        }

        public static Vector2Int FloorToVector2Int(this Vector3 v)
        {
            return new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
        }

        public static Vector2 SetX(this Vector2 v, float x)
        {
            return new Vector2(x, v.y);
        }

        public static Vector2 SetY(this Vector2 v, float y)
        {
            return new Vector2(v.x, y);
        }

        public static Vector3 SetX(this Vector3 v, float x)
        {
            return new Vector3(x, v.y, v.z);
        }

        public static Vector3 SetY(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        public static Vector3 SetZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }
    }
}
