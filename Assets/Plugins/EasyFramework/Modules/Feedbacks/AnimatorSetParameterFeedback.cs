using System;
using EasyFramework.ToolKit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.Modules
{
    [AddFeedbackMenu("动画控制器/设置参数")]
    public class AnimatorSetParameterFeedback : AbstractFeedback
    {
        [FoldoutGroup("参数设置")]
        [InfoBoxEx("动画控制器不能为空！", InfoMessageType.Error, VisibleIf = nameof(ShowTargetError))]
        [LabelText("动画控制器")]
        public Animator Target;

        [FoldoutGroup("参数设置")]
        [LabelText("参数名称")]
        public string ParameterName;

        [FoldoutGroup("参数设置")]
        [LabelText("参数类型")]
        public AnimatorControllerParameterType ParameterType;

        [FoldoutGroup("参数设置")]
        [LabelText("参数设置（Int）")]
        [ShowIf(nameof(ParameterType), AnimatorControllerParameterType.Int)]
        public int IntToSet;

        [FoldoutGroup("参数设置")]
        [LabelText("参数设置（Float）")]
        [ShowIf(nameof(ParameterType), AnimatorControllerParameterType.Float)]
        public float FloatToSet;
        
        [FoldoutGroup("参数设置")]
        [LabelText("参数设置（Bool）")]
        [ShowIf(nameof(ParameterType), AnimatorControllerParameterType.Bool)]
        public bool BoolToSet;

        public override string Tip => "设置指定动画控制器的参数";

        private bool ShowTargetError => Target == null;

        private int _paramId;

        protected override void OnFeedbackInit()
        {
            _paramId = Animator.StringToHash(ParameterName);
        }

        protected override void OnFeedbackPlay()
        {
            switch (ParameterType)
            {
                case AnimatorControllerParameterType.Int:
                    Target.SetInteger(_paramId, IntToSet);
                    break;
                case AnimatorControllerParameterType.Float:
                    Target.SetFloat(_paramId, FloatToSet);
                    break;
                case AnimatorControllerParameterType.Bool:
                    Target.SetBool(_paramId, BoolToSet);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    Target.SetTrigger(_paramId);
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
