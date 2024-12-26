using UnityEngine;

namespace EasyGameFramework
{
    public static class VectorExtension
    {
        public static Vector2 ToCrossDirection(this Vector2 v)
        {
            v = v.normalized;

            var absX = Mathf.Abs(v.x);
            var absY = Mathf.Abs(v.y);

            if (v.x > 0f)
            {
                if (v.y > 0f)
                {
                    return v.x > v.y ? Vector2.right : Vector2.up;
                }
                else
                {
                    return v.x > absY ? Vector2.right : Vector2.down;
                }
            }
            else
            {
                if (v.y > 0f)
                {
                    return absX > v.y ? Vector2.left : Vector2.up;
                }
                else
                {
                    return absX > absY ? Vector2.left : Vector2.down;
                }
            }
        }

        public static bool IsVertical(this Vector2 v)
        {
            v = ToCrossDirection(v);
            return v == Vector2.up || v == Vector2.down;
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
    }
}
