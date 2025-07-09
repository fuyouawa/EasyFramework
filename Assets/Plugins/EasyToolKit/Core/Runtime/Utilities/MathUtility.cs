using UnityEngine;

namespace EasyToolKit.Core
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

        #region Random

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

        public static Vector2 GetRandomPointInCircle(Vector2 center, float radius, float radiusThickness = 1f)
        {
            var invalidR = radius * (1f - radiusThickness);

            float angle = Random.Range(0f, Mathf.PI * 2);
            float r = Random.Range(invalidR, radius);

            float x = r * Mathf.Cos(angle);
            float y = r * Mathf.Sin(angle);

            return new Vector2(center.x + x, center.y + y);
        }

        #endregion

        #region Rectangle

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
            vertices[1] = new Vector2(-size.x / 2, size.y / 2); // 左上角
            vertices[2] = new Vector2(size.x / 2, size.y / 2); // 右上角
            vertices[3] = new Vector2(size.x / 2, -size.y / 2); // 右下角

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

        #endregion

        public static Vector3 QuadraticBezierCurve(float normalizedTime, Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint)
        {
            return Mathf.Pow(1 - normalizedTime, 2) * startPoint +
                   2 * (1 - normalizedTime) * normalizedTime * controlPoint +
                   Mathf.Pow(normalizedTime, 2) * endPoint;
        }

        public static float CalculateQuadraticBezierLength(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint, int segments = 20)
        {
            float length = 0f;
            Vector3 prevPoint = startPoint;

            for (int i = 1; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector3 point = Mathf.Pow(1 - t, 2) * startPoint +
                                2 * (1 - t) * t * controlPoint +
                                Mathf.Pow(t, 2) * endPoint;
                length += Vector3.Distance(prevPoint, point);
                prevPoint = point;
            }

            return length;
        }
    }
}
