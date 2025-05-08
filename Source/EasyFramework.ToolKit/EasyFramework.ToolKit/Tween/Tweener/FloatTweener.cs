using EasyFramework.Core;

namespace EasyFramework.ToolKit
{
    public class FloatTweener : AbstractTweener<float>
    {
        protected override float GetDistance(float startValue, float endValue)
        {
            return (endValue - startValue).Abs();
        }

        protected override float GetCurrentValue(float time, float duration, float startValue, float endValue)
        {
            var t = MathUtility.Remap(time, 0f, duration, 0f, 1f);
            var res = TweenUtility.EaseValue(EaseMode, t, startValue, endValue);
            return res;
        }
    }
}
