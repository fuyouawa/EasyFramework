using System;
using EasyFramework.Core;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public interface IInterpolator
    {
        bool CanInterpolate(Type valueType);

        object Interpolate(Type valueType, object startValue, object endValue, float t);
    }

    public enum BezierControlPointRelativeTo
    {
        None,
        StartPoint,
        EndPoint
    }

    public class QuadraticBezierInterpolator : IInterpolator
    {
        private Vector3 _controlPoint;
        private BezierControlPointRelativeTo _controlPointRelativeTo;

        public QuadraticBezierInterpolator SetControlPoint(Vector3 point)
        {
            _controlPoint = point;
            return this;
        }

        public QuadraticBezierInterpolator SetControlPointRelative(BezierControlPointRelativeTo relativeTo)
        {
            _controlPointRelativeTo = relativeTo;
            return this;
        }

        bool IInterpolator.CanInterpolate(Type valueType)
        {
            return valueType == typeof(Vector2) || valueType == typeof(Vector3);
        }

        object IInterpolator.Interpolate(Type valueType, object startValue, object endValue, float t)
        {
            Vector3 startPoint;
            Vector3 endPoint;
            if (valueType == typeof(Vector2))
            {
                startPoint = (Vector2)startValue;
                endPoint = (Vector2)endValue;
            }
            else
            {
                startPoint = (Vector3)startValue;
                endPoint = (Vector3)endValue;
            }

            var controlPoint = _controlPoint;
            switch (_controlPointRelativeTo)
            {
                case BezierControlPointRelativeTo.None:
                    break;
                case BezierControlPointRelativeTo.StartPoint:
                    controlPoint += startPoint;
                    break;
                case BezierControlPointRelativeTo.EndPoint:
                    controlPoint += endPoint;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var curPos = MathUtility.QuadraticBezierCurve(startPoint, endPoint, controlPoint, t);

            if (valueType == typeof(Vector2))
            {
                return curPos.ToVec2();
            }
            else
            {
                return curPos;
            }
        }
    }
}
