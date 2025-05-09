using System;
using EasyFramework.Core;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public interface ISecondaryEase
    {
        bool CanEase(Type valueType);

        T Ease<T>(T startValue, T endValue, float easedTime) => (T)Ease(typeof(T), startValue, endValue, easedTime);

        object Ease(Type valueType, object startValue, object endValue, float easedTime);
    }

    public enum BezierControlPointRelativeTo
    {
        None,
        StartPoint,
        EndPoint
    }

    public class QuadraticBezierSecondaryEase : ISecondaryEase
    {
        private Vector3 _controlPoint;
        private BezierControlPointRelativeTo _controlPointRelativeTo;

        public QuadraticBezierSecondaryEase SetControlPoint(Vector3 point)
        {
            _controlPoint = point;
            return this;
        }

        public QuadraticBezierSecondaryEase SetBezierControlRelative(BezierControlPointRelativeTo relativeTo)
        {
            _controlPointRelativeTo = relativeTo;
            return this;
        }

        bool ISecondaryEase.CanEase(Type valueType)
        {
            return valueType == typeof(Vector2) || valueType == typeof(Vector3);
        }

        object ISecondaryEase.Ease(Type valueType, object startValue, object endValue, float easedTime)
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

            var curPos = MathUtility.QuadraticBezierCurve(startPoint, endPoint, controlPoint, easedTime);

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

    public static class SecondaryEaseFactory
    {
        public static QuadraticBezierSecondaryEase QuadraticBezier()
        {
            return new QuadraticBezierSecondaryEase();
        }
    }
}
