using System;
using EasyFramework.Core;
using UnityEngine;
using UnityEngine.Rendering;

namespace EasyFramework.ToolKit
{
    internal static class TweenUtility
    {
        public static float EaseTime(EaseType easeType, float time)
        {
            // 根据不同的Ease类型应用不同的插值函数
            switch (easeType)
            {
                case EaseType.Linear:
                    // 线性插值，直接返回时间值
                    return time;

                #region Sine (正弦插值)

                case EaseType.InSine:
                    // 进入正弦插值
                    return 1f - Mathf.Cos((time * Mathf.PI) / 2f);
                case EaseType.OutSine:
                    // 离开正弦插值
                    return Mathf.Sin((time * Mathf.PI) / 2f);
                case EaseType.InOutSine:
                    // 正弦插值（进出）
                    return -(Mathf.Cos(Mathf.PI * time) - 1f) / 2f;

                #endregion

                #region Quad (二次插值)

                case EaseType.InQuad:
                    // 进入二次插值
                    return time * time;
                case EaseType.OutQuad:
                    // 离开二次插值
                    return 1f - (1f - time) * (1f - time);
                case EaseType.InOutQuad:
                    // 二次插值（进出）
                    return time < 0.5f ? 2f * time * time : 1f - Mathf.Pow(-2f * time + 2f, 2f) / 2f;

                #endregion

                #region Cubic (三次插值)

                case EaseType.InCubic:
                    // 进入三次插值
                    return time * time * time;
                case EaseType.OutCubic:
                    // 离开三次插值
                    return 1f - Mathf.Pow(1f - time, 3f);
                case EaseType.InOutCubic:
                    // 三次插值（进出）
                    return time < 0.5f ? 4f * time * time * time : 1f - Mathf.Pow(-2f * time + 2f, 3f) / 2f;

                #endregion

                #region Quart (四次插值)

                case EaseType.InQuart:
                    // 进入四次插值
                    return time * time * time * time;
                case EaseType.OutQuart:
                    // 离开四次插值
                    return 1f - Mathf.Pow(1f - time, 4f);
                case EaseType.InOutQuart:
                    // 四次插值（进出）
                    return time < 0.5f ? 8f * time * time * time * time : 1f - Mathf.Pow(-2f * time + 2f, 4f) / 2f;

                #endregion

                #region Quint (五次插值)

                case EaseType.InQuint:
                    // 进入五次插值
                    return time * time * time * time * time;
                case EaseType.OutQuint:
                    // 离开五次插值
                    return 1f - Mathf.Pow(1f - time, 5f);
                case EaseType.InOutQuint:
                    // 五次插值（进出）
                    return time < 0.5f
                        ? 16f * time * time * time * time * time
                        : 1f - Mathf.Pow(-2f * time + 2f, 5f) / 2f;

                #endregion

                #region Back (回弹插值)

                case EaseType.InBack:
                {
                    // 进入回弹插值
                    const float c1 = 1.70158f;
                    return c1 * time * time * time - c1 * time * time;
                }
                case EaseType.OutBack:
                {
                    // 离开回弹插值
                    const float c1 = 1.70158f;
                    float t1 = time - 1f;
                    return 1f + c1 * t1 * t1 * t1 + c1 * t1 * t1;
                }
                case EaseType.InOutBack:
                {
                    // 回弹插值（进出）
                    const float c1 = 1.70158f * 1.525f;
                    return time < 0.5f
                        ? (Mathf.Pow(2f * time, 2f) * ((c1 + 1f) * 2f * time - c1)) / 2f
                        : (Mathf.Pow(2f * time - 2f, 2f) * ((c1 + 1f) * (time * 2f - 2f) + c1) + 2f) / 2f;
                }

                #endregion

                #region Elastic (弹性插值)

                case EaseType.InElastic:
                {
                    // 进入弹性插值
                    if (time == 0f || time.Approximately(1f)) return time;
                    const float c = (2f * Mathf.PI) / 0.3f;
                    return -Mathf.Pow(2f, 10f * time - 10f) * Mathf.Sin((time * 10f - 10.75f) * c);
                }

                case EaseType.OutElastic:
                {
                    // 离开弹性插值
                    if (time == 0f || time.Approximately(1f)) return time;
                    const float c = (2f * Mathf.PI) / 0.3f;
                    return Mathf.Pow(2f, -10f * time) * Mathf.Sin((time * 10f - 0.75f) * c) + 1f;
                }

                case EaseType.InOutElastic:
                {
                    // 弹性插值（进出）
                    if (time == 0f || time.Approximately(1f)) return time;
                    const float c = (2f * Mathf.PI) / 0.45f;
                    return time < 0.5f
                        ? -(Mathf.Pow(2f, 20f * time - 10f) * Mathf.Sin((20f * time - 11.125f) * c)) / 2f
                        : (Mathf.Pow(2f, -20f * time + 10f) * Mathf.Sin((20f * time - 11.125f) * c)) / 2f + 1f;
                }

                    break;

                #endregion

                #region Bounce (弹跳插值)

                case EaseType.InBounce:
                    // 进入弹跳插值
                    return 1f - BounceEaseOut(1f - time);
                case EaseType.OutBounce:
                    // 离开弹跳插值
                    return BounceEaseOut(time);
                case EaseType.InOutBounce:
                    // 弹跳插值（进出）
                    return time < 0.5f
                        ? (1f - BounceEaseOut(1f - 2f * time)) / 2f
                        : (1f + BounceEaseOut(2f * time - 1f)) / 2f;

                #endregion

                default:
                    // 未定义的Ease类型，抛出异常
                    throw new ArgumentOutOfRangeException();
            }
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
