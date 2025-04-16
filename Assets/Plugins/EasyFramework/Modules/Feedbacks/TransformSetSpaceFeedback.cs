using EasyFramework.ToolKit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.Modules
{
    [AddFeedbackMenu("变换/设置空间属性")]
    public class TransformSetSpaceFeedback : AbstractFeedback
    {
        [FoldoutGroup("变换设置")]
        [InfoBoxEx("目标变换不能为空！", InfoMessageType.Error, nameof(ShowTargetError))]
        [LabelText("目标变换")]
        public Transform Target;
        
        [FoldoutGroup("坐标设置")]
        [LabelText("修改坐标")]
        public bool ModifyPosition = true;

        [FoldoutGroup("坐标设置")]
        [ShowIf(nameof(ModifyPosition))]
        [LabelText("局部坐标")]
        public bool LocalPositionToSet = true;

        [FoldoutGroup("坐标设置")]
        [ShowIf(nameof(ModifyPosition))]
        [LabelText("坐标")]
        public Vector3 PositionToSet;
        
        [FoldoutGroup("旋转设置")]
        [LabelText("修改旋转")]
        public bool ModifyRotation = true;

        [FoldoutGroup("旋转设置")]
        [ShowIf(nameof(ModifyRotation))]
        [LabelText("局部旋转")]
        public bool LocalRotationToSet = true;

        [FoldoutGroup("旋转设置")]
        [ShowIf(nameof(ModifyRotation))]
        [LabelText("旋转")]
        public Vector3 RotationToSet;
        
        [FoldoutGroup("缩放设置")]
        [LabelText("修改缩放")]
        public bool ModifyScale = true;

        [FoldoutGroup("缩放设置")]
        [ShowIf(nameof(ModifyScale))]
        [LabelText("局部缩放")]
        public Vector3 LocalScaleToSet = Vector3.one;
        
        [FoldoutGroup("父级设置")]
        [LabelText("修改父级")]
        public bool ModifyParent;

        [FoldoutGroup("父级设置")]
        [ShowIf(nameof(ModifyParent))]
        [LabelText("父级")]
        public Transform ParentToSet;

        [FoldoutGroup("父级设置")]
        [ShowIf(nameof(ModifyParent))]
        [LabelText("保持世界坐标")]
        public bool WorldPositionStay = false;
        
        public override string Tip => "修改Transform的各种属性";

        private bool ShowTargetError => Target == null;

        protected override void OnFeedbackPlay()
        {
            if (ModifyPosition)
            {
                if (LocalPositionToSet)
                {
                    Target.localPosition = PositionToSet;
                }
                else
                {
                    Target.position = PositionToSet;
                }
            }

            if (ModifyRotation)
            {
                if (LocalRotationToSet)
                {
                    Target.localRotation = Quaternion.Euler(RotationToSet);
                }
                else
                {
                    Target.rotation = Quaternion.Euler(RotationToSet);
                }
            }

            if (ModifyScale)
            {
                Target.localScale = LocalScaleToSet;
            }

            if (ModifyParent)
            {
                Target.SetParent(ParentToSet, WorldPositionStay);
            }
        }

        protected override void OnFeedbackStop()
        {
        }
    }
}
