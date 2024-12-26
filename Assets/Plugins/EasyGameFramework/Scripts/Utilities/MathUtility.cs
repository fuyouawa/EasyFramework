using UnityEngine;

namespace EasyGameFramework
{
    public static class MathUtility
    {
        /// <summary>
        /// 将一个在[A, B]区间的x, 映射到[C, D]区间
        /// </summary>
        /// <returns></returns>
        public static float Remap(float x, float A, float B, float C, float D)
        {
            return C + (x - A) / (B - A) * (D - C);
        }

        public static bool Approximately(Quaternion a, Quaternion b, float similarityThreshold = 0.99f)
        {
            var dot = Quaternion.Dot(a, b);
            var threshold = Mathf.Clamp(similarityThreshold, 0f, 1f);
            return dot >= threshold;
        }

        public static bool Approximately(Vector3 a, Vector3 b, float similarityThreshold = 0.99f)
        {
            var distance = Vector3.Distance(a, b);
            var threshold = Mathf.Clamp(similarityThreshold, 0f, 1f);
            return distance <= 1 - threshold;
        }

        public static Vector2 GetRandomPointInRectangle(Rect rect)
        {
            return GetRandomPointInRotatedRectangle(rect, 0f);
        }

        public static Vector2 GetRandomPointInRectangle(Vector2 center, Vector2 size)
        {
            return GetRandomPointInRotatedRectangle(center, size, 0f);
        }

        public static Vector2 GetRandomPointInRotatedRectangle(Rect rect, float angle)
        {
            return GetRandomPointInRotatedRectangle(rect.center, rect.size, angle);
        }
        

        public static Vector3 GetRandomBetweenTwoVector2(Vector2 min, Vector2 max)
        {
            return new Vector3(
                Random.Range(min.x, max.x),
                Random.Range(min.y, max.y));
        }

        public static Vector3 GetRandomBetweenTwoVector3(Vector3 min, Vector3 max)
        {
            return new Vector3(
                Random.Range(min.x, max.x),
                Random.Range(min.y, max.y),
                Random.Range(min.z, max.z));
        }

        public static Vector2 GetRandomPointInRotatedRectangle(Vector2 center, Vector2 size, float angle)
        {
            // 在未旋转的矩形内生成一个随机点
            float randomX = Random.Range(-size.x / 2, size.x / 2); // 宽度的随机值
            float randomY = Random.Range(-size.y / 2, size.y / 2); // 高度的随机值
            Vector2 randomPoint = new Vector2(randomX, randomY);

            if (!Mathf.Approximately(angle, 0))
            {
                // 将随机点绕矩形中心旋转rotationAngle度
                float radians = angle * Mathf.Deg2Rad;
                float cosAngle = Mathf.Cos(radians);
                float sinAngle = Mathf.Sin(radians);

                // 旋转后的点（相对于中心）
                randomX = randomPoint.x * cosAngle - randomPoint.y * sinAngle;
                randomY = randomPoint.x * sinAngle + randomPoint.y * cosAngle;
            }

            randomPoint = new Vector2(center.x + randomX, center.y + randomY);

            return randomPoint;
        }

        /// <summary>
        /// 获取旋转后矩形的四个顶点
        /// </summary>
        /// <returns>[左下角, 左上角, 右上角, 右下角]</returns>
        public static Vector2[] GetVerticesOfRotatedRectangle(Rect rect, float angle)
        {
            return GetVerticesOfRotatedRectangle(rect.center, rect.size, angle);
        }

        /// <summary>
        /// 获取旋转后矩形的四个顶点
        /// </summary>
        /// <returns>[左下角, 左上角, 右上角, 右下角]</returns>
        public static Vector2[] GetVerticesOfRotatedRectangle(Vector2 center, Vector2 size, float angle)
        {
            // 计算矩形的四个顶点（未旋转的情况）
            Vector2[] vertices = new Vector2[4];
            vertices[0] = new Vector2(-size.x / 2, -size.y / 2); // 左下角
            vertices[1] = new Vector2(-size.x / 2, size.y / 2);  // 左上角
            vertices[2] = new Vector2(size.x / 2, size.y / 2);   // 右上角
            vertices[3] = new Vector2(size.x / 2, -size.y / 2);  // 右下角

            if (!Mathf.Approximately(angle, 0))
            {
                // 旋转角度转换为弧度
                float radians = angle * Mathf.Deg2Rad;
                float cosAngle = Mathf.Cos(radians);
                float sinAngle = Mathf.Sin(radians);

                // 对每个顶点应用旋转变换
                for (int i = 0; i < vertices.Length; i++)
                {
                    float x = vertices[i].x;
                    float y = vertices[i].y;

                    // 旋转公式
                    float rotatedX = x * cosAngle - y * sinAngle;
                    float rotatedY = x * sinAngle + y * cosAngle;

                    // 更新顶点位置，相对于中心进行平移
                    vertices[i] = new Vector2(rotatedX + center.x, rotatedY + center.y);
                }
            }
            else
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    float x = vertices[i].x;
                    float y = vertices[i].y;

                    vertices[i] = new Vector2(x + center.x, y + center.y);
                }
            }

            return vertices;
        }

        public static Vector2 GetRandomPointInCircle(Vector2 center, float radius, float radiusThickness = 1f)
        {
            var invalidR = radius * (1f - radiusThickness);
                    
            float angle = Random.Range(0f, Mathf.PI * 2);
            float r = Random.Range(invalidR, radius);

            float x = r * Mathf.Cos(angle);
            float y = r * Mathf.Sin(angle);

            return new Vector2(center.x + x, center.y + y);
        }

        public static Vector2 QuadraticBezierCurve(Vector2 startPoint, Vector2 endPoint, Vector2 controlPoint, float t)
        {
            return Mathf.Pow(1 - t, 2) * startPoint +
                    2 * (1 - t) * t * controlPoint +
                    Mathf.Pow(t, 2) * endPoint;
        }
        
        public static float Round(this float value, int decimals)
        {
            return (float)System.Math.Round(value, decimals);
        }
        public static float Round(this float value)
        {
            return (float)System.Math.Round(value);
        }
    }
}
