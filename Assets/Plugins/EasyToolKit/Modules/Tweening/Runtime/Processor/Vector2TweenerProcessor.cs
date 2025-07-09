using System;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Tweening
{
    public class Vector2LinearTweenerProcessor : AbstractTweenerProcessor<Vector2, LinearTweenerEffect>
    {
        protected override Vector2 GetRelativeValue(Vector2 value, Vector2 relative)
        {
            return value + relative;
        }

        private float _distance;
        private Vector2 _direction;
        protected override void OnInit()
        {
            _distance = Vector2.Distance(Context.StartValue, Context.EndValue);
            _direction = (Context.EndValue - Context.StartValue).normalized;
        }
        
        protected override float GetDistance()
        {
            return _distance;
        }

        protected override Vector2 OnProcess(float normalizedTime)
        {
            var curDist = Mathf.Lerp(0, _distance, normalizedTime);
            return Context.StartValue + curDist * _direction;
        }
    }

    public class Vector2BezierTweenerProcessor : AbstractTweenerProcessor<Vector2, BezierTweenerEffect>
    {
        protected override Vector2 GetRelativeValue(Vector2 value, Vector2 relative)
        {
            return value + relative;
        }

        private Vector2 _controlPoint;
        protected override void OnInit()
        {
            _controlPoint = Context.Effect.ControlPoint.ToVec2();
            switch (Context.Effect.ControlPointRelativeTo)
            {
                case BezierControlPointRelativeTo.None:
                    break;
                case BezierControlPointRelativeTo.StartPoint:
                    _controlPoint += Context.StartValue;
                    break;
                case BezierControlPointRelativeTo.EndPoint:
                    _controlPoint += Context.EndValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override float GetDistance()
        {
            return MathUtility.CalculateQuadraticBezierLength(Context.StartValue, _controlPoint, Context.EndValue);
        }

        protected override Vector2 OnProcess(float normalizedTime)
        {
            var curPos = MathUtility.QuadraticBezierCurve(normalizedTime, Context.StartValue, _controlPoint, Context.EndValue);
            return curPos;
        }
    }
}
