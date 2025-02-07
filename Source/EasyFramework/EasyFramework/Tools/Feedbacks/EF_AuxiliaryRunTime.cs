using System.Collections;
using UnityEngine;

namespace EasyFramework.Feedbacks
{
    [AddEasyFeedbackMenu("辅助/运行指定时间")]
    public class EF_AuxiliaryRunTime : AbstractEasyFeedback
    {
        public enum Modes
        {
            Frame,
            Seconds
        }

        public Modes Mode;
        public int Frame;
        public float Seconds;

        public override string Tip => "运行指定时间，可以通过启用“阻塞”来实现暂停的效果";

        protected override IEnumerator Pause => PauseCo();

        private IEnumerator PauseCo()
        {
            if (Mode == Modes.Frame)
            {
                for (int i = 0; i < Frame; i++)
                {
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(Seconds);
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
