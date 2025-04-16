using System.Collections;
using EasyFramework.ToolKit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.Modules
{
    [AddFeedbackMenu("光照/光照控制")]
    public class LightControlFeedback : AbstractFeedback
    {
        [FoldoutGroup("光照控制")]
        [LabelText("光照对象")]
        public Light TargetLight;

        [FoldoutGroup("光照控制")]
        [LabelText("持续时间")]
        public float Duration = 0.2f;

        [FoldoutGroup("光照控制")]
        [LabelText("停止时禁用")]
        public bool DisableOnStop = false;

        [FoldoutGroup("光照控制")]
        [LabelText("使用相对数值")]
        public bool RelativeValues = true;
        
        [FoldoutGroup("光照颜色")]
        [LabelText("修改颜色")]
        public bool ModifyColor = true;

        [FoldoutGroup("光照颜色")]
        [ShowIf(nameof(ModifyColor))]
        [LabelText("颜色")]
        public GradientColor Color = new GradientColor(UnityEngine.Color.white);
        
        [FoldoutGroup("光照强度")]
        [LabelText("修改强度")]
        public bool ModifyIntensity = true;
        
        [FoldoutGroup("光照强度")]
        [ShowIf(nameof(ModifyIntensity))]
        [LabelText("强度")]
        public CurveValue Intensity = new CurveValue(
            true, 1f,
            new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0)));
        
        [FoldoutGroup("光照范围")]
        [LabelText("修改范围")]
        public bool ModifyRange = true;
        
        [FoldoutGroup("光照范围")]
        [ShowIf(nameof(ModifyRange))]
        [LabelText("范围")]
        public CurveValue Range = new CurveValue(
            true, 10f,
            new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0)));

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

            if (Color.IsGradient || Intensity.IsCurve || Range.IsCurve)
            {
                StartCoroutine(LightSequence());
            }
            else
            {
                if (ModifyIntensity)
                {
                    TargetLight.intensity = Intensity.Value;
                }

                if (ModifyColor)
                {
                    TargetLight.color = Color.Color;
                }

                if (ModifyRange)
                {
                    TargetLight.range = Range.Value;
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
