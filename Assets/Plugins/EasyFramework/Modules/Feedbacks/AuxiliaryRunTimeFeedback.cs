using System.Collections;
using EasyFramework.ToolKit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.Modules
{
    [AddFeedbackMenu("辅助/运行指定时间")]
    public class AuxiliaryRunTimeFeedback : AbstractFeedback
    {
        public enum Modes
        {
            Frame,
            Seconds
        }

        [FoldoutGroup("时间设置")]
        [LabelText("模式")]
        public Modes Mode;

        [FoldoutGroup("时间设置")]
        [ShowIf(nameof(Mode), Modes.Frame)]
        [LabelText("帧")]
        public int Frame;

        [FoldoutGroup("时间设置")]
        [ShowIf(nameof(Mode), Modes.Seconds)]
        [LabelText("秒")]
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
