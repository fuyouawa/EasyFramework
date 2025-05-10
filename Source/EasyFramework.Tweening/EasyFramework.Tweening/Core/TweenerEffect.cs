using UnityEngine;

namespace EasyFramework.Tweening
{
    public interface ITweenerEffect
    {
    }

    public class LinearTweenerEffect : ITweenerEffect
    {
    }

    public enum BezierControlPointRelativeTo
    {
        None,
        StartPoint,
        EndPoint
    }

    public class BezierTweenerEffect : ITweenerEffect
    {
        public Vector3 ControlPoint;
        public BezierControlPointRelativeTo ControlPointRelativeTo;

        public BezierTweenerEffect SetControlPoint(Vector3 point)
        {
            ControlPoint = point;
            return this;
        }

        public BezierTweenerEffect SetControlPointRelative(BezierControlPointRelativeTo relativeTo)
        {
            ControlPointRelativeTo = relativeTo;
            return this;
        }
    }

    public static class Effect
    {
        public static LinearTweenerEffect Linear()
        {
            return new LinearTweenerEffect();
        }

        public static BezierTweenerEffect Bezier()
        {
            return new BezierTweenerEffect();
        }
    }
}
