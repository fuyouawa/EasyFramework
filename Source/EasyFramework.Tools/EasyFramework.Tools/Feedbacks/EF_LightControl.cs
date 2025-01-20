using System.Collections;
using EasyFramework.Utilities;
using UnityEngine;

namespace EasyFramework.Tools
{
    [AddEasyFeedbackMenu("光照/光照控制")]
    public class EF_LightControl : AbstractEasyFeedback
    {
        public Light TargetLight;
        public float Duration = 0.2f;
        public bool DisableOnStop = false;
        public bool RelativeValues = true;

        public bool ModifyColor = true;
        public FlexibleColor Color = new FlexibleColor(UnityEngine.Color.white);

        public bool ModifyIntensity = true;
        public FlexibleFloat Intensity = new FlexibleFloat(
            FlexibleModes.Overtime,
            1f,
            new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0)),
            0f, 1f);

        public bool ModifyRange = true;
        public FlexibleFloat Range = new FlexibleFloat(
            FlexibleModes.Overtime,
            10f,
            new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0)),
            0f, 10f);

        public override string Tip => "光照控制";

        protected float _initialRange;
        protected float _initialShadowStrength;
        protected float _initialIntensity;
        protected Coroutine _coroutine;
        protected Color _initialColor;

        public override float GetDuration()
        {
            return Duration;
        }

        protected override void OnFeedbackInit()
        {
            if (TargetLight == null)
                return;

            _initialRange = TargetLight.range;
            _initialShadowStrength = TargetLight.shadowStrength;
            _initialIntensity = TargetLight.intensity;
            _initialColor = TargetLight.color;
        }

        protected override void OnFeedbackPlay()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            TargetLight.enabled = true;

            if (Color.IsOvertime || Intensity.IsOvertime || Range.IsOvertime)
            {
                StartCoroutine(LightSequence());
            }
            else
            {
                if (ModifyIntensity)
                {
                    TargetLight.intensity = Intensity.InstantOfValue;
                }

                if (ModifyColor)
                {
                    TargetLight.color = Color.InstantOfColor;
                }

                if (ModifyRange)
                {
                    TargetLight.range = Range.InstantOfValue;
                }
            }
        }

        protected override void OnFeedbackStop()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            if (DisableOnStop)
            {
                TargetLight.enabled = false;
            }
        }

        protected virtual IEnumerator LightSequence()
        {
            var totalTime = 0f;
            while (totalTime <= Duration)
            {
                SetLightValues(totalTime);
                totalTime += Time.deltaTime;
                yield return null;
            }
        }

        protected virtual void SetLightValues(float time)
        {
            var intensity = Intensity.Evaluate(time, Duration);
            var range = Intensity.Evaluate(time, Duration);
            var color = Color.Evaluate(time, Duration);

            if (RelativeValues)
            {
                intensity += _initialIntensity;
                range += _initialRange;
            }

            if (ModifyIntensity)
            {
                TargetLight.intensity = intensity;
            }

            if (ModifyRange)
            {
                TargetLight.range = range;
            }

            if (ModifyColor)
            {
                TargetLight.color = color;
            }
        }
    }
}
