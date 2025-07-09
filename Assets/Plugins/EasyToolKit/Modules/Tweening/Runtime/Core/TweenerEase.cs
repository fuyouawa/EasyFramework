using EasyToolKit.Core;
using System;
using UnityEngine;

namespace EasyToolKit.Tweening
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

    public class InExponentialTwennerEase : ITweenerEase
    {
        private float _power = 2;

        public InExponentialTwennerEase SetPow(float pow)
        {
            _power = pow;
            return this;
        }

        float ITweenerEase.EaseTime(float time)
        {
            return Mathf.Pow(time, _power);
        }
    }

    public class OutExponentialTwennerEase : ITweenerEase
    {
        private float _power = 2;

        public OutExponentialTwennerEase SetPow(float pow)
        {
            _power = pow;
            return this;
        }

        float ITweenerEase.EaseTime(float time)
        {
            return 1f - Mathf.Pow(1f - time, _power);
        }
    }

    public class InOutExponentialTwennerEase : ITweenerEase
    {
        private float _power = 2;

        public InOutExponentialTwennerEase SetPow(float pow)
        {
            _power = pow;
            return this;
        }

        float ITweenerEase.EaseTime(float time)
        {
            return time < 0.5f
                ? Mathf.Pow(2f * time, _power) / 2f
                : 1f - Mathf.Pow(2f - 2f * time, _power) / 2f;
        }
    }

    public static class Ease
    {
        /// <summary>
        /// 线性匀速过渡，无加速度。
        /// </summary>
        public static ITweenerEase Linear()
        {
            return new GenericTweenerEase(time => time);
        }

        /// <summary>
        /// 使用正弦函数起始缓慢，逐渐加速。
        /// </summary>
        public static ITweenerEase InSine()
        {
            return new GenericTweenerEase(time => 1f - Mathf.Cos((time * Mathf.PI) / 2f));
        }

        /// <summary>
        /// 使用正弦函数起始快速，逐渐减速。
        /// </summary>
        public static ITweenerEase OutSine()
        {
            return new GenericTweenerEase(time => Mathf.Sin((time * Mathf.PI) / 2f));
        }

        /// <summary>
        /// 使用正弦函数在开始和结束时都较缓慢，中间加速。
        /// </summary>
        public static ITweenerEase InOutSine()
        {
            return new GenericTweenerEase(time => -(Mathf.Cos(Mathf.PI * time) - 1f) / 2f);
        }

        /// <summary>
        /// 二次函数缓动，起始缓慢。
        /// </summary>
        public static ITweenerEase InQuad()
        {
            return new InExponentialTwennerEase().SetPow(2f);
        }

        /// <summary>
        /// 二次函数缓动，结束缓慢。
        /// </summary>
        public static ITweenerEase OutQuad()
        {
            return new OutExponentialTwennerEase().SetPow(2f);
        }

        /// <summary>
        /// 二次函数缓动，开始和结束都缓慢，中间加速。
        /// </summary>
        public static ITweenerEase InOutQuad()
        {
            return new InOutExponentialTwennerEase().SetPow(2f);
        }

        /// <summary>
        /// 三次函数缓动，起始缓慢。
        /// </summary>
        public static ITweenerEase InCubic()
        {
            return new InExponentialTwennerEase().SetPow(3f);
        }

        /// <summary>
        /// 三次函数缓动，结束缓慢。
        /// </summary>
        public static ITweenerEase OutCubic()
        {
            return new OutExponentialTwennerEase().SetPow(3f);
        }

        /// <summary>
        /// 三次函数缓动，开始和结束都缓慢，中间加速。
        /// </summary>
        public static ITweenerEase InOutCubic()
        {
            return new InOutExponentialTwennerEase().SetPow(3f);
        }

        /// <summary>
        /// 四次函数缓动，起始缓慢。
        /// </summary>
        public static ITweenerEase InQuart()
        {
            return new InExponentialTwennerEase().SetPow(4f);
        }

        /// <summary>
        /// 四次函数缓动，结束缓慢。
        /// </summary>
        public static ITweenerEase OutQuart()
        {
            return new OutExponentialTwennerEase().SetPow(4f);
        }

        /// <summary>
        /// 四次函数缓动，开始和结束都缓慢，中间加速。
        /// </summary>
        public static ITweenerEase InOutQuart()
        {
            return new InOutExponentialTwennerEase().SetPow(4f);
        }

        /// <summary>
        /// 五次函数缓动，起始缓慢。
        /// </summary>
        public static ITweenerEase InQuint()
        {
            return new InExponentialTwennerEase().SetPow(5f);
        }

        /// <summary>
        /// 五次函数缓动，结束缓慢。
        /// </summary>
        public static ITweenerEase OutQuint()
        {
            return new OutExponentialTwennerEase().SetPow(5f);
        }

        /// <summary>
        /// 五次函数缓动，开始和结束都缓慢，中间加速。
        /// </summary>
        public static ITweenerEase InOutQuint()
        {
            return new InOutExponentialTwennerEase().SetPow(5f);
        }

        /// <summary>
        /// 指数函数缓动，起始缓慢。
        /// </summary>
        /// <param name="power">指数次方</param>
        public static ITweenerEase InExponential(float power)
        {
            return new InExponentialTwennerEase().SetPow(power);
        }

        /// <summary>
        /// 指数函数缓动，结束缓慢。
        /// </summary>
        /// <param name="power">指数次方</param>
        public static ITweenerEase OutExponential(float power)
        {
            return new OutExponentialTwennerEase().SetPow(power);
        }

        /// <summary>
        /// 指数函数缓动，开始和结束都缓慢，中间加速。
        /// </summary>
        /// <param name="power">指数次方</param>
        public static ITweenerEase InOutExponential(float power)
        {
            return new InOutExponentialTwennerEase().SetPow(power);
        }

        /// <summary>
        /// 向后拉动再向前加速的动画。
        /// </summary>
        public static ITweenerEase InBack()
        {
            const float c1 = 1.70158f;
            return new GenericTweenerEase(time => c1 * time * time * time - c1 * time * time);
        }

        /// <summary>
        /// 向前滑动并超出目标后回弹的动画。
        /// </summary>
        public static ITweenerEase OutBack()
        {
            const float c1 = 1.70158f;
            return new GenericTweenerEase(time =>
            {
                float t1 = time - 1f;
                return 1f + c1 * t1 * t1 * t1 + c1 * t1 * t1;
            });
        }

        /// <summary>
        /// 起始和终点都有回弹的动画。
        /// </summary>
        public static ITweenerEase InOutBack()
        {
            const float c1 = 1.70158f * 1.525f;
            return new GenericTweenerEase(time => time < 0.5f
                ? (Mathf.Pow(2f * time, 2f) * ((c1 + 1f) * 2f * time - c1)) / 2f
                : (Mathf.Pow(2f * time - 2f, 2f) * ((c1 + 1f) * (time * 2f - 2f) + c1) + 2f) / 2f);
        }

        /// <summary>
        /// 起始具有弹簧震荡效果，逐渐进入动画。
        /// </summary>
        public static ITweenerEase InElastic()
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
        public static ITweenerEase OutElastic()
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
        public static ITweenerEase InOutElastic()
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
        public static ITweenerEase InBounce()
        {
            return new GenericTweenerEase(time => 1f - BounceEaseOut(1f - time));
        }

        /// <summary>
        /// 动画结束像球落地一样反弹，逐渐停止。
        /// </summary>
        public static ITweenerEase OutBounce()
        {
            return new GenericTweenerEase(time => BounceEaseOut(time));
        }

        /// <summary>
        /// 动画开始和结束都带有弹跳效果。
        /// </summary>
        public static ITweenerEase InOutBounce()
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
