using EasyFramework.Core;
using System;
using UnityEngine;

namespace EasyFramework.Tweening
{
    public class Vector3LinearTweenerProcessor : AbstractTweenerProcessor<Vector3, LinearTweenerEffect>
    {
        protected override Vector3 GetRelativeValue(Vector3 value, Vector3 relative)
        {
            return value + relative;
        }

        private float _distance;
        private Vector3 _direction;
        protected override void OnInit()
        {
            _distance = Vector3.Distance(Context.StartValue, Context.EndValue);
            _direction = (Context.EndValue - Context.StartValue).normalized;
        }
        
        protected override float GetDistance()
        {
            return _distance;
        }

        protected override Vector3 OnProcess(float time)
        {
            var curDist = Mathf.Lerp(0, _distance, time);
            return Context.StartValue + curDist * _direction;
        }
    }

    public class Vector3BezierTweenerProcessor : AbstractTweenerProcessor<Vector3, BezierTweenerEffect>
    {
        protected override Vector3 GetRelativeValue(Vector3 value, Vector3 relative)
        {
            return value + relative;
        }

        private Vector3 _controlPoint;
        protected override void OnInit()
        {
            _controlPoint = Context.Effect.ControlPoint;
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
            return MathUtility.CalculateQuadraticBezierLength(Context.StartValue, Context.EndValue, _controlPoint);
        }

        protected override Vector3 OnProcess(float time)
        {
            var curPos = MathUtility.QuadraticBezierCurve(Context.StartValue, Context.EndValue, _controlPoint, time);
            return curPos;
        }
    }
}
