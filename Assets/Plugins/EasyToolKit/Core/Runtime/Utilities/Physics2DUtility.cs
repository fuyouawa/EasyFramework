using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

namespace EasyToolKit.Core
{
    public class RaycastHit2DEqualityComparer : IEqualityComparer<RaycastHit2D>
    {
        public bool Equals(RaycastHit2D x, RaycastHit2D y)
        {
            return x.collider == y.collider;
        }

        public int GetHashCode(RaycastHit2D obj)
        {
            return obj.collider.GetHashCode();
        }
    }
    public class RaycastHit2DComparer : IComparer<RaycastHit2D>
    {
        public int Compare(RaycastHit2D x, RaycastHit2D y)
        {
            var distanceComparison = x.distance.CompareTo(y.distance);
            if (distanceComparison != 0) return distanceComparison;
            return x.fraction.CompareTo(y.fraction);
        }
    }

    public static class Physics2DUtility
    {
        public static RaycastHit2D DirectedSegmentCast(Vector2 begin, Vector2 end, Vector2 direction,
            int layerMask)
        {
            if (direction == Vector2.up)
            {
                if (begin.y > end.y)
                    (begin, end) = (end, begin);
            }
            else if (direction == Vector2.down)
            {
                if (begin.y < end.y)
                    (begin, end) = (end, begin);
            }
            else if (direction == Vector2.left)
            {
                if (begin.x > end.x)
                    (begin, end) = (end, begin);
            }
            else if (direction == Vector2.right)
            {
                if (begin.x < end.x)
                    (begin, end) = (end, begin);
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Unknow direction '{direction}'");
            }

            return Physics2D.Raycast(
                begin,
                (end - begin).normalized,
                Vector2.Distance(begin, end),
                layerMask);
        }

        public static RaycastHit2D DirectedRectCast(
            Vector2 center,
            Vector2 size,
            float density,
            Vector2 direction,
            int layerMask)
        {
            var halfSize = size / 2;

            var p1 = new Vector2(center.x - halfSize.x, center.y - halfSize.y);
            var p2 = new Vector2(center.x + halfSize.x, center.y - halfSize.y);
            var p3 = new Vector2(center.x + halfSize.x, center.y + halfSize.y);
            var p4 = new Vector2(center.x - halfSize.x, center.y + halfSize.y);

            var total = new List<RaycastHit2D>();

            if (direction.IsVertical())
            {
                total.Add(DirectedSegmentCast(p2, p3, direction, layerMask));
                total.Add(DirectedSegmentCast(p4, p1, direction, layerMask));

                int gridX = Mathf.FloorToInt(size.x / density);
                var stepX = size.x / gridX;
                for (int i = 1; i < gridX; i++)
                {
                    var x = p1.x + i * stepX;
                    total.Add(DirectedSegmentCast(new Vector2(x, p1.y), new Vector2(x, p4.y), direction, layerMask));
                }
            }
            else
            {
                total.Add(DirectedSegmentCast(p1, p2, direction, layerMask));
                total.Add(DirectedSegmentCast(p3, p4, direction, layerMask));

                int gridY = Mathf.FloorToInt(size.y / density);
                var stepY = size.y / gridY;
                for (int i = 1; i < gridY; i++)
                {
                    var y = p1.y + i * stepY;
                    total.Add(DirectedSegmentCast(new Vector2(p1.x, y), new Vector2(p2.x, y), direction, layerMask));
                }
            }

            if (total.Count > 0)
            {
                total = total.Where(x => x.collider != null)
                    .Distinct(new RaycastHit2DEqualityComparer())
                    .OrderBy(h => h.distance)
                    .ToList();

                return total.FirstOrDefault();
            }

            return new RaycastHit2D();
        }
    }
}
