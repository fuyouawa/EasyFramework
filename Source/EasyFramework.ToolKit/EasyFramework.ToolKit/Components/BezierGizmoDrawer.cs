using EasyFramework.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public class BezierGizmoDrawer : MonoBehaviour
    {
        [Title("绑定")]
        [LabelText("起始点")]
        public Transform StartPoint;
        [LabelText("控制点")]
        public Transform ControlPoint;
        [LabelText("结束点")]
        public Transform EndPoint;

        [Title("调试设置")]
        [LabelText("曲线颜色")]
        public Color CurveColor = Color.red;
        [LabelText("辅助线颜色")]
        public Color AuxiliaryLineColor = Color.gray;
        [LabelText("线段数量")]
        public int Segments = 20;

        private void OnDrawGizmos()
        {
            if (StartPoint == null || ControlPoint == null || EndPoint == null)
                return;

            Vector3 prevPoint = StartPoint.position;

            for (int i = 1; i <= Segments; i++)
            {
                float t = i / (float)Segments;

                Vector3 point = MathUtility.QuadraticBezierCurve(t, StartPoint.position, ControlPoint.position, EndPoint.position);
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(prevPoint, point);

                prevPoint = point;
            }

            // 可选：绘制点和辅助线
            Gizmos.color = CurveColor;
            Gizmos.DrawSphere(StartPoint.position, 0.05f);
            Gizmos.DrawSphere(ControlPoint.position, 0.05f);
            Gizmos.DrawSphere(EndPoint.position, 0.05f);

            Gizmos.color = AuxiliaryLineColor;
            Gizmos.DrawLine(StartPoint.position, ControlPoint.position);
            Gizmos.DrawLine(ControlPoint.position, EndPoint.position);
        }
    }
}
