using System.Collections;
using Sirenix.OdinInspector;

namespace EasyGameFramework
{
    [EasyFeedbackHelper("运行指定帧数")]
    [AddEasyFeedbackMenu("辅助/运行指定帧数")]
    public class EF_AuxiliaryRunFrame : AbstractEasyFeedback
    {
        [FoldoutGroup("Run Frame")]
        public float Frame;

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
