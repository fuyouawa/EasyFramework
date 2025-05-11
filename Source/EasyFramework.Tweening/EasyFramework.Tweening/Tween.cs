using System;
using UnityEngine;

namespace EasyFramework.Tweening
{
    public static class Tween
    {
        public static Tweener To(Type valueType, TweenGetter getter, TweenSetter setter, object endValue,
            float duration)
        {
            var tweener = TweenManager.Instance.GetTweener();
            tweener.Apply(valueType, getter, setter, endValue);
            tweener.SetDuration(duration);
            return tweener;
        }

        public static Tweener To<T>(TweenGetter<T> getter, TweenSetter<T> setter, T endValue, float duration)
        {
            return To(typeof(T), () => getter(), val => setter((T)val), endValue, duration);
        }

        public static TweenSequence Sequence()
        {
            return TweenManager.Instance.GetSequence();
        }

        public static TweenCallback Callback(Action callback)
        {
            return TweenManager.Instance.GetCallback().AddCallback(callback);
        }

        /// <summary>
        /// 创建一个指定持续时间的间隔
        /// </summary>
        /// <param name="duration">间隔时间（秒）</param>
        /// <returns>间隔Tween对象</returns>
        public static TweenInterval Interval(float duration)
        {
            var interval = TweenManager.Instance.GetInterval();
            interval.SetDuration(duration);
            return interval;
        }

        /// <summary>
        /// <para>获取持续时间，返回null代表此时无法判断具体持续时间。</para>
        /// <para>如果是TweenSequence，只有运行完成了一次，才能确定那一次运行持续的时间，否则将始终返回null。</para>
        /// </summary>
        /// <param name="tween"></param>
        /// <returns></returns>
        public static float? Duration(AbstractTween tween)
        {
            return tween.GetActualDuration();
        }

        /// <summary>
        /// <para>通过ID获取Tween的持续时间，返回null代表此时无法判断具体持续时间。</para>
        /// <para>如果是TweenSequence，只有运行完成了一次，才能确定那一次运行持续的时间，否则将始终返回null。</para>
        /// </summary>
        /// <param name="id">Tween的ID</param>
        /// <returns>如果找到对应的Tween则返回其持续时间，否则返回null</returns>
        public static float? Duration(string id)
        {
            var tween = GetById(id);
            return tween?.GetActualDuration();
        }

        public static bool IsPlaying(AbstractTween tween)
        {
            return tween.CurrentState == TweenState.Playing || tween.CurrentState == TweenState.DelayAfterPlay;
        }

        /// <summary>
        /// 通过ID检查Tween是否正在播放
        /// </summary>
        /// <param name="id">Tween的ID</param>
        /// <returns>如果找到对应的Tween且正在播放则返回true，否则返回false</returns>
        public static bool IsPlaying(string id)
        {
            var tween = GetById(id);
            return tween != null && IsPlaying(tween);
        }

        public static bool IsActive(AbstractTween tween)
        {
            return IsPlaying(tween) || tween.CurrentState == TweenState.Idle;
        }

        /// <summary>
        /// 通过ID检查Tween是否处于活动状态
        /// </summary>
        /// <param name="id">Tween的ID</param>
        /// <returns>如果找到对应的Tween且处于活动状态则返回true，否则返回false</returns>
        public static bool IsActive(string id)
        {
            var tween = GetById(id);
            return tween != null && IsActive(tween);
        }

        public static void Kill(AbstractTween tween)
        {
            tween.PendingKill = true;
            if (tween.OwnerSequence == null)
            {
                TweenController.Instance.AddPendingKill(tween);
            }
        }

        /// <summary>
        /// 通过ID终止Tween
        /// </summary>
        /// <param name="id">Tween的ID</param>
        public static void Kill(string id)
        {
            var tween = GetById(id);
            if (tween != null)
            {
                Kill(tween);
            }
        }

        /// <summary>
        /// 通过ID获取Tween实例
        /// </summary>
        /// <param name="id">Tween的ID</param>
        /// <returns>如果找到则返回对应的Tween实例，否则返回null</returns>
        public static AbstractTween GetById(string id)
        {
            return TweenController.Instance.GetTweenById(id);
        }
    }
}
