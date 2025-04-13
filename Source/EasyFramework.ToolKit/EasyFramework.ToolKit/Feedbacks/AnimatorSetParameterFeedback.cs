using System;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    [AddFeedbackMenu("动画控制器/设置参数")]
    public class AnimatorSetParameterFeedback : AbstractFeedback
    {
        public enum ParameterTypes
        {
            Int,
            Float,
            Bool
        }

        public Animator TargetAnimator;
        public string ParameterName;
        public ParameterTypes ParameterType;
        public int IntToSet;
        public float FloatToSet;
        public bool BoolToSet;

        public override string Tip => "设置指定动画控制器的参数";

        private int _paramId;

        protected override void OnFeedbackInit()
        {
            _paramId = Animator.StringToHash(ParameterName);
        }

        protected override void OnFeedbackPlay()
        {
            switch (ParameterType)
            {
                case ParameterTypes.Int:
                    TargetAnimator.SetInteger(_paramId, IntToSet);
                    break;
                case ParameterTypes.Float:
                    TargetAnimator.SetFloat(_paramId, FloatToSet);
                    break;
                case ParameterTypes.Bool:
                    TargetAnimator.SetBool(_paramId, BoolToSet);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnFeedbackStop()
        {
        }
    }
}
