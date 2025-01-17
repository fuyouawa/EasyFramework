using System;
using System.Collections;
using EasyFramework.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

 namespace EasyFramework.Tools
{
    [AddEasyFeedbackMenu("光照/光照控制")]
    public class EF_LightControl : AbstractEasyFeedback
    {
        public enum Modes { OverTime, Instant }

        [FoldoutGroup("Light")]
        public Light BoundLight;
        [FoldoutGroup("Light")]
        public Modes Mode = Modes.OverTime;
        [FoldoutGroup("Light")]
        [HideIf("Mode", Modes.Instant)]
        public float Duration = 0.2f;
        [FoldoutGroup("Light")]
        public bool DisableOnStop = false;
        [FoldoutGroup("Light")]
        public bool RelativeValues = true;
        [FoldoutGroup("Light")]
        public bool AllowAdditivePlays = false;

        [FoldoutGroup("Color")]
        public bool ModifyColor = true;
        [FoldoutGroup("Color")]
        [ShowIf("Mode", Modes.OverTime)]
        public Gradient ColorOverTime;
        [FoldoutGroup("Color")]
        [ShowIf("Mode", Modes.Instant)]
        public Color InstantColor = Color.red;

        [FoldoutGroup("Intensity")]
        public bool ModifyIntensity = true;
        [FoldoutGroup("Intensity")]
        [ShowIf("Mode", Modes.OverTime)]
        public AnimationCurve IntensityCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0));
        [FoldoutGroup("Intensity")]
        [ShowIf("Mode", Modes.OverTime)]
        public float RemapIntensityZero = 0f;
        [FoldoutGroup("Intensity")]
        [ShowIf("Mode", Modes.OverTime)]
        public float RemapIntensityOne = 1f;
        [FoldoutGroup("Intensity")]
        [ShowIf("Mode", Modes.Instant)]
        public float InstantIntensity = 1f;

        [FoldoutGroup("Range")]
        public bool ModifyRange = true;
        [FoldoutGroup("Range")]
        [ShowIf("Mode", Modes.OverTime)]
        public AnimationCurve RangeCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0));
        [FoldoutGroup("Range")]
        [ShowIf("Mode", Modes.OverTime)]
        public float RemapRangeZero = 0f;
        [FoldoutGroup("Range")]
        [ShowIf("Mode", Modes.OverTime)]
        public float RemapRangeOne = 1f;
        [FoldoutGroup("Range")]
        [ShowIf("Mode", Modes.Instant)]
        public float InstantRange = 10f;
        
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
            if (BoundLight == null)
                return;

            _initialRange = BoundLight.range;
            _initialShadowStrength = BoundLight.shadowStrength;
            _initialIntensity = BoundLight.intensity;
            _initialColor = BoundLight.color;
        }

        protected override void OnFeedbackPlay()
        {
            if (_coroutine != null && !AllowAdditivePlays)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            BoundLight.enabled = true;
            switch (Mode)
            {
                case Modes.OverTime:
                    _coroutine = StartCoroutine(LightSequence());
                    break;
                case Modes.Instant:
                    BoundLight.intensity = InstantIntensity;
                    BoundLight.range = InstantRange;
                    if (ModifyColor)
                        BoundLight.color = InstantColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
                BoundLight.enabled = false;
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
            var intensity = IntensityCurve.EvaluateWithRemap(time, Duration, RemapIntensityZero, RemapIntensityOne);
            var range = RangeCurve.EvaluateWithRemap(time, Duration, RemapRangeZero, RemapRangeOne);

            var color = ColorOverTime.EvaluateWithRemap(time, Duration);

            if (RelativeValues)
            {
                intensity += _initialIntensity;
                range += _initialRange;
            }
            if (ModifyIntensity)
            {
                BoundLight.intensity = intensity;
            }
            if (ModifyRange)
            {
                BoundLight.range = range;
            }
            if (ModifyColor)
            {
                BoundLight.color = color;
            }
        }
    }
}
