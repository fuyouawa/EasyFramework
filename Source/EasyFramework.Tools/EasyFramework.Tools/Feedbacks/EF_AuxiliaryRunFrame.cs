using System.Collections;

namespace EasyFramework.Tools
{
    [AddEasyFeedbackMenu("辅助/运行指定帧数")]
    public class EF_AuxiliaryRunFrame : AbstractEasyFeedback
    {
        public float Frame;
        
        public override string Tip => "运行指定帧数";

        protected override IEnumerator Pause => RunFrameCo();

        private IEnumerator RunFrameCo()
        {
            for (int i = 0; i < Frame; i++)
            {
                yield return null;
            }
        }

        protected override void OnFeedbackPlay()
        {
            
        }

        protected override void OnFeedbackStop()
        {
            
        }
    }
}
