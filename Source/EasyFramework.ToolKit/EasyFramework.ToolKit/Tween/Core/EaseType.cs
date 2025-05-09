using UnityEngine;

namespace EasyFramework.ToolKit
{
    /// <summary>
    /// 缓动函数类型枚举，用于控制插值动画的速率变化曲线。
    /// </summary>
    public enum EaseType
    {
        /// <summary>
        /// 线性匀速过渡，无加速度。
        /// </summary>
        Linear,

        /// <summary>
        /// 使用正弦函数起始缓慢，逐渐加速。
        /// </summary>
        InSine,

        /// <summary>
        /// 使用正弦函数起始快速，逐渐减速。
        /// </summary>
        OutSine,

        /// <summary>
        /// 使用正弦函数在开始和结束时都较缓慢，中间加速。
        /// </summary>
        InOutSine,

        /// <summary>
        /// 二次函数缓动，起始缓慢。
        /// </summary>
        InQuad,

        /// <summary>
        /// 二次函数缓动，结束缓慢。
        /// </summary>
        OutQuad,

        /// <summary>
        /// 二次函数缓动，开始和结束都缓慢，中间加速。
        /// </summary>
        InOutQuad,

        /// <summary>
        /// 三次函数缓动，起始缓慢。
        /// </summary>
        InCubic,

        /// <summary>
        /// 三次函数缓动，结束缓慢。
        /// </summary>
        OutCubic,

        /// <summary>
        /// 三次函数缓动，开始和结束都缓慢。
        /// </summary>
        InOutCubic,

        /// <summary>
        /// 四次函数缓动，起始缓慢。
        /// </summary>
        InQuart,

        /// <summary>
        /// 四次函数缓动，结束缓慢。
        /// </summary>
        OutQuart,

        /// <summary>
        /// 四次函数缓动，开始和结束都缓慢。
        /// </summary>
        InOutQuart,

        /// <summary>
        /// 五次函数缓动，起始缓慢。
        /// </summary>
        InQuint,

        /// <summary>
        /// 五次函数缓动，结束缓慢。
        /// </summary>
        OutQuint,

        /// <summary>
        /// 五次函数缓动，开始和结束都缓慢。
        /// </summary>
        InOutQuint,

        /// <summary>
        /// 向后拉动再向前加速的动画，具有“超前”效果。
        /// </summary>
        InBack,

        /// <summary>
        /// 向前滑动并“超出”目标后回弹的动画。
        /// </summary>
        OutBack,

        /// <summary>
        /// 起始和终点都有回弹的动画，形成前后“超出”效果。
        /// </summary>
        InOutBack,

        /// <summary>
        /// 起始具有弹簧震荡效果，逐渐进入动画。
        /// </summary>
        InElastic,

        /// <summary>
        /// 动画结束时产生弹簧震荡回弹效果。
        /// </summary>
        OutElastic,

        /// <summary>
        /// 动画开始和结束都具有弹簧震荡效果。
        /// </summary>
        InOutElastic,

        /// <summary>
        /// 动画开始像球落地一样反弹，逐渐收敛。
        /// </summary>
        InBounce,

        /// <summary>
        /// 动画结束像球落地一样反弹，逐渐停止。
        /// </summary>
        OutBounce,

        /// <summary>
        /// 动画开始和结束都带有弹跳效果。
        /// </summary>
        InOutBounce,
    }

    /// <summary>
    /// 次级缓动函数类型枚举，用于为<see cref="EaseType"/>增加次级效果。
    /// </summary>
    public enum SecondaryEaseType
    {
        None,

        /// <summary>
        /// <para>二次贝塞尔缓动曲线，用于通过控制点实现自定义缓动轨迹。</para>
        /// <para>需要配合 <see cref="QuadraticBezierEaseConfig"/> 配置使用</para>
        /// <para>仅适用于支持 <c>Vector</c> 类型的 Tweener。</para>
        /// </summary>
        QuadraticBezier,
    }

    public interface IEaseConfig
    {
    }

    public interface ISecondaryEaseConfig
    {
    }


    public class QuadraticBezierEaseConfig : ISecondaryEaseConfig
    {
        public Vector3 ControlPoint;
        public bool RelativeToStartPoint;

        public QuadraticBezierEaseConfig(Vector3 controlPoint, bool relativeToStartPoint = true)
        {
            ControlPoint = controlPoint;
            RelativeToStartPoint = relativeToStartPoint;
        }
    }
}
