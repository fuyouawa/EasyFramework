using UnityEngine;

namespace EasyFramework.ToolKit
{
    [AddEasyFeedbackMenu("变换/设置属性")]
    public class EF_TransformSetProperties : AbstractEasyFeedback
    {
        public Transform Target;

        public bool ModifyTransform = true;
        public bool Local;
        public Vector3 PositionToSet;
        public Vector3 RotationToSet;
        public Vector3 LocalScaleToSet;

        public bool ModifyParent = true;
        public Transform ParentToSet;
        public bool WorldPositionStay = true;
        
        public override string Tip => "修改Transform的各种属性";

        protected override void OnFeedbackPlay()
        {
            if (ModifyParent)
            {
                Target.SetParent(ParentToSet, WorldPositionStay);
            }

            if (ModifyTransform)
            {
                if (Local)
                {
                    Target.SetLocalPositionAndRotation(PositionToSet, Quaternion.Euler(RotationToSet));
                }
                else
                {
                    Target.SetPositionAndRotation(PositionToSet, Quaternion.Euler(RotationToSet));
                }

                Target.localScale = LocalScaleToSet;
            }
        }

        protected override void OnFeedbackStop()
        {
        }
    }
}
