using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace EasyFramework.ToolKit
{
    internal static class TweenUtility
    {
        // 根据指定的插值方式计算插值值
        public static float EaseValue(Ease ease, float time, float startValue, float endValue)
        {
            float easedT;

            // 根据不同的Ease类型应用不同的插值函数
            switch (ease)
            {
                case Ease.Linear:
                    // 线性插值，直接返回时间值
                    easedT = time;
                    break;

                #region Sine (正弦插值)

                case Ease.InSine:
                    // 进入正弦插值
                    easedT = 1f - Mathf.Cos((time * Mathf.PI) / 2f);
                    break;
                case Ease.OutSine:
                    // 离开正弦插值
                    easedT = Mathf.Sin((time * Mathf.PI) / 2f);
                    break;
                case Ease.InOutSine:
                    // 正弦插值（进出）
                    easedT = -(Mathf.Cos(Mathf.PI * time) - 1f) / 2f;
                    break;

                #endregion

                #region Quad (二次插值)

                case Ease.InQuad:
                    // 进入二次插值
                    easedT = time * time;
                    break;
                case Ease.OutQuad:
                    // 离开二次插值
                    easedT = 1f - (1f - time) * (1f - time);
                    break;
                case Ease.InOutQuad:
                    // 二次插值（进出）
                    easedT = time < 0.5f ? 2f * time * time : 1f - Mathf.Pow(-2f * time + 2f, 2f) / 2f;
                    break;

                #endregion

                #region Cubic (三次插值)

                case Ease.InCubic:
                    // 进入三次插值
                    easedT = time * time * time;
                    break;
                case Ease.OutCubic:
                    // 离开三次插值
                    easedT = 1f - Mathf.Pow(1f - time, 3f);
                    break;
                case Ease.InOutCubic:
                    // 三次插值（进出）
                    easedT = time < 0.5f ? 4f * time * time * time : 1f - Mathf.Pow(-2f * time + 2f, 3f) / 2f;
                    break;

                #endregion

                #region Quart (四次插值)

                case Ease.InQuart:
                    // 进入四次插值
                    easedT = time * time * time * time;
                    break;
                case Ease.OutQuart:
                    // 离开四次插值
                    easedT = 1f - Mathf.Pow(1f - time, 4f);
                    break;
                case Ease.InOutQuart:
                    // 四次插值（进出）
                    easedT = time < 0.5f ? 8f * time * time * time * time : 1f - Mathf.Pow(-2f * time + 2f, 4f) / 2f;
                    break;

                #endregion

                #region Quint (五次插值)

                case Ease.InQuint:
                    // 进入五次插值
                    easedT = time * time * time * time * time;
                    break;
                case Ease.OutQuint:
                    // 离开五次插值
                    easedT = 1f - Mathf.Pow(1f - time, 5f);
                    break;
                case Ease.InOutQuint:
                    // 五次插值（进出）
                    easedT = time < 0.5f
                        ? 16f * time * time * time * time * time
                        : 1f - Mathf.Pow(-2f * time + 2f, 5f) / 2f;
                    break;

                #endregion

                #region Back (回弹插值)

                case Ease.InBack:
                {
                    // 进入回弹插值
                    const float c1 = 1.70158f;
                    easedT = c1 * time * time * time - c1 * time * time;
                }
                    break;
                case Ease.OutBack:
                {
                    // 离开回弹插值
                    const float c1 = 1.70158f;
                    float t1 = time - 1f;
                    easedT = 1f + c1 * t1 * t1 * t1 + c1 * t1 * t1;
                }
                    break;
                case Ease.InOutBack:
                {
                    // 回弹插值（进出）
                    const float c1 = 1.70158f * 1.525f;
                    easedT = time < 0.5f
                        ? (Mathf.Pow(2f * time, 2f) * ((c1 + 1f) * 2f * time - c1)) / 2f
                        : (Mathf.Pow(2f * time - 2f, 2f) * ((c1 + 1f) * (time * 2f - 2f) + c1) + 2f) / 2f;
                }
                    break;

                #endregion

                #region Elastic (弹性插值)

                case Ease.InElastic:
                    // 进入弹性插值
                    if (time == 0f || time == 1f) easedT = time;
                    else
                    {
                        const float c = (2f * Mathf.PI) / 0.3f;
                        easedT = -Mathf.Pow(2f, 10f * time - 10f) * Mathf.Sin((time * 10f - 10.75f) * c);
                    }

                    break;
                case Ease.OutElastic:
                    // 离开弹性插值
                    if (time == 0f || time == 1f) easedT = time;
                    else
                    {
                        const float c = (2f * Mathf.PI) / 0.3f;
                        easedT = Mathf.Pow(2f, -10f * time) * Mathf.Sin((time * 10f - 0.75f) * c) + 1f;
                    }

                    break;
                case Ease.InOutElastic:
                    // 弹性插值（进出）
                    if (time == 0f || time == 1f) easedT = time;
                    else
                    {
                        const float c = (2f * Mathf.PI) / 0.45f;
                        easedT = time < 0.5f
                            ? -(Mathf.Pow(2f, 20f * time - 10f) * Mathf.Sin((20f * time - 11.125f) * c)) / 2f
                            : (Mathf.Pow(2f, -20f * time + 10f) * Mathf.Sin((20f * time - 11.125f) * c)) / 2f + 1f;
                    }

                    break;

                #endregion

                #region Bounce (弹跳插值)

                case Ease.InBounce:
                    // 进入弹跳插值
                    easedT = 1f - BounceEaseOut(1f - time);
                    break;
                case Ease.OutBounce:
                    // 离开弹跳插值
                    easedT = BounceEaseOut(time);
                    break;
                case Ease.InOutBounce:
                    // 弹跳插值（进出）
                    easedT = time < 0.5f
                        ? (1f - BounceEaseOut(1f - 2f * time)) / 2f
                        : (1f + BounceEaseOut(2f * time - 1f)) / 2f;
                    break;

                #endregion

                default:
                    // 未定义的Ease类型，抛出异常
                    throw new ArgumentOutOfRangeException();
            }

            // 使用Mathf.Lerp进行最终的插值计算
            float result = Mathf.Lerp(startValue, endValue, easedT);
            return result;
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
