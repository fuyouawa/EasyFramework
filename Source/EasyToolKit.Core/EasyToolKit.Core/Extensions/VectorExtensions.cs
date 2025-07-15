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

        public static Vector3 ToVec3(this Vector2 v, float z = 0f)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector2 ToVec2(this Vector3 v)
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
            return new Vector2(v.x.Round(), v.y.Round());
        }


        public static Vector3 Round(this Vector3 v)
        {
            return new Vector3(v.x.Round(), v.y.Round(), v.z.Round());
        }


        public static Vector2 Round(this Vector2 v, int decimals)
        {
            return new Vector2(v.x.Round(decimals), v.y.Round(decimals));
        }


        public static Vector3 Round(this Vector3 v, int decimals)
        {
            return new Vector3(v.x.Round(decimals), v.y.Round(decimals), v.z.Round(decimals));
        }

        public static Vector2 ToVec2(this Vector2Int v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector3 ToVec3(this Vector2Int v, int z = 0)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector3 ToVec3(this Vector3Int v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static Vector2 ToVec2(this Vector3Int v)
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

        public static Vector2Int RoundToVec2Int(this Vector2 v)
        {
            return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }

        public static Vector3Int RoundToVec3Int(this Vector2 v)
        {
            return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }

        public static Vector3Int RoundToVec3Int(this Vector3 v)
        {
            return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
        }

        public static Vector2Int RoundToVec2Int(this Vector3 v)
        {
            return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }

        public static Vector2Int FloorToVec2Int(this Vector2 v)
        {
            return new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
        }

        public static Vector3Int FloorToVec3Int(this Vector2 v)
        {
            return new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
        }

        public static Vector3Int FloorToVec3Int(this Vector3 v)
        {
            return new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));
        }

        public static Vector2Int FloorToVec2Int(this Vector3 v)
        {
            return new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
        }

        public static Vector2 NewX(this Vector2 v, float x)
        {
            return new Vector2(x, v.y);
        }

        public static Vector2 NewY(this Vector2 v, float y)
        {
            return new Vector2(v.x, y);
        }

        public static Vector3 NewX(this Vector3 v, float x)
        {
            return new Vector3(x, v.y, v.z);
        }

        public static Vector3 NewY(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        public static Vector3 NewZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }
    }
}
