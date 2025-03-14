using UnityEngine;

namespace EasyFramework.ToolKit
{
    [AddEasyFeedbackMenu("动画控制器/触发参数")]
    public class EF_AnimatorTriggerParameter : AbstractEasyFeedback
    {
        public Animator TargetAnimator;
        public string ParameterName;
        
        public override string Tip => "触发指定动画控制器的参数";

        private int _paramId;

        protected override void OnFeedbackInit()
        {
            _paramId = Animator.StringToHash(ParameterName);
        }

        protected override void OnFeedbackPlay()
        {
            TargetAnimator.SetTrigger(_paramId);
        }

        protected override void OnFeedbackStop()
        {
        }
    }
}
