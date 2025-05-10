using EasyFramework.Core;
using System;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public interface ITweenerEase
    {
        float EaseTime(float time);
    }

    public class GenericTweenerEase : ITweenerEase
    {
        private readonly Func<float, float> _easeTime;

        internal GenericTweenerEase(Func<float, float> easeTime)
        {
            _easeTime = easeTime;
        }

        float ITweenerEase.EaseTime(float time)
        {
            return _easeTime(time);
        }
    }

    public static class TweenerEase
    {
        /// <summary>
        /// 线性匀速过渡，无加速度。
        /// </summary>
        public static GenericTweenerEase Linear()
        {
            return new GenericTweenerEase(time => time);
        }

        /// <summary>
        /// 使用正弦函数起始缓慢，逐渐加速。
        /// </summary>
        public static GenericTweenerEase InSine()
        {
            return new GenericTweenerEase(time => 1f - Mathf.Cos((time * Mathf.PI) / 2f));
        }

        /// <summary>
        /// 使用正弦函数起始快速，逐渐减速。
        /// </summary>
        public static GenericTweenerEase OutSine()
        {
            return new GenericTweenerEase(time => Mathf.Sin((time * Mathf.PI) / 2f));
        }

        /// <summary>
        /// 使用正弦函数在开始和结束时都较缓慢，中间加速。
        /// </summary>
        public static GenericTweenerEase InOutSine()
        {
            return new GenericTweenerEase(time => -(Mathf.Cos(Mathf.PI * time) - 1f) / 2f);
        }

        /// <summary>
        /// 二次函数缓动，起始缓慢。
        /// </summary>
        public static GenericTweenerEase InQuad()
        {
            return new GenericTweenerEase(time => time * time);
        }

        /// <summary>
        /// 二次函数缓动，结束缓慢。
        /// </summary>
        public static GenericTweenerEase OutQuad()
        {
            return new GenericTweenerEase(time => 1f - (1f - time) * (1f - time));
        }

        /// <summary>
        /// 二次函数缓动，开始和结束都缓慢，中间加速。
        /// </summary>
        public static GenericTweenerEase InOutQuad()
        {
            return new GenericTweenerEase(time => time < 0.5f ? 2f * time * time : 1f - Mathf.Pow(-2f * time + 2f, 2f) / 2f);
        }

        /// <summary>
        /// 向后拉动再向前加速的动画，具有“超前”效果。
        /// </summary>
        public static GenericTweenerEase InBack()
        {
            const float c1 = 1.70158f;
            return new GenericTweenerEase(time => c1 * time * time * time - c1 * time * time);
        }

        /// <summary>
        /// 向前滑动并“超出”目标后回弹的动画。
        /// </summary>
        public static GenericTweenerEase OutBack()
        {
            const float c1 = 1.70158f;
            return new GenericTweenerEase(time =>
            {
                float t1 = time - 1f;
                return 1f + c1 * t1 * t1 * t1 + c1 * t1 * t1;
            });
        }

        /// <summary>
        /// 起始和终点都有回弹的动画，形成前后“超出”效果。
        /// </summary>
        public static GenericTweenerEase InOutBack()
        {
            const float c1 = 1.70158f * 1.525f;
            return new GenericTweenerEase(time => time < 0.5f
                ? (Mathf.Pow(2f * time, 2f) * ((c1 + 1f) * 2f * time - c1)) / 2f
                : (Mathf.Pow(2f * time - 2f, 2f) * ((c1 + 1f) * (time * 2f - 2f) + c1) + 2f) / 2f);
        }

        /// <summary>
        /// 起始具有弹簧震荡效果，逐渐进入动画。
        /// </summary>
        public static GenericTweenerEase InElastic()
        {
            return new GenericTweenerEase(time =>
            {
                if (time == 0f || time.Approximately(1f)) return time;
                const float c = (2f * Mathf.PI) / 0.3f;
                return -Mathf.Pow(2f, 10f * time - 10f) * Mathf.Sin((time * 10f - 10.75f) * c);
            });
        }

        /// <summary>
        /// 动画结束时产生弹簧震荡回弹效果。
        /// </summary>
        public static GenericTweenerEase OutElastic()
        {
            return new GenericTweenerEase(time =>
            {
                if (time == 0f || time.Approximately(1f)) return time;
                const float c = (2f * Mathf.PI) / 0.3f;
                return Mathf.Pow(2f, -10f * time) * Mathf.Sin((time * 10f - 0.75f) * c) + 1f;
            });
        }

        /// <summary>
        /// 动画开始和结束都具有弹簧震荡效果。
        /// </summary>
        public static GenericTweenerEase InOutElastic()
        {
            return new GenericTweenerEase(time =>
            {
                if (time == 0f || time.Approximately(1f)) return time;
                const float c = (2f * Mathf.PI) / 0.45f;
                return time < 0.5f
                    ? -(Mathf.Pow(2f, 20f * time - 10f) * Mathf.Sin((20f * time - 11.125f) * c)) / 2f
                    : (Mathf.Pow(2f, -20f * time + 10f) * Mathf.Sin((20f * time - 11.125f) * c)) / 2f + 1f;
            });
        }

        /// <summary>
        /// 动画开始像球落地一样反弹，逐渐收敛。
        /// </summary>
        public static GenericTweenerEase InBounce()
        {
            return new GenericTweenerEase(time => 1f - BounceEaseOut(1f - time));
        }

        /// <summary>
        /// 动画结束像球落地一样反弹，逐渐停止。
        /// </summary>
        public static GenericTweenerEase OutBounce()
        {
            return new GenericTweenerEase(time => BounceEaseOut(time));
        }

        /// <summary>
        /// 动画开始和结束都带有弹跳效果。
        /// </summary>
        public static GenericTweenerEase InOutBounce()
        {
            return new GenericTweenerEase(time => time < 0.5f
                ? (1f - BounceEaseOut(1f - 2f * time)) / 2f
                : (1f + BounceEaseOut(2f * time - 1f)) / 2f);
        }

        // 弹跳效果的计算方法
        private static float BounceEaseOut(float time)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (time < 1f / d1)
                return n1 * time * time;
            else if (time < 2f / d1)
            {
                time -= 1.5f / d1;
                return n1 * time * time + 0.75f;
            }
            else if (time < 2.5f / d1)
            {
                time -= 2.25f / d1;
                return n1 * time * time + 0.9375f;
            }
            else
            {
                time -= 2.625f / d1;
                return n1 * time * time + 0.984375f;
            }
        }
    }
}
