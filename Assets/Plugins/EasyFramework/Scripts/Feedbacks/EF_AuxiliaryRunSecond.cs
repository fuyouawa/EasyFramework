using Sirenix.OdinInspector;

namespace EasyGameFramework
{
    [EasyFeedbackHelper("运行指定时间(秒)")]
    [AddEasyFeedbackMenu("辅助/运行指定时间(秒)")]
    public class EF_AuxiliaryRunSecond : AbstractEasyFeedback
    {
        [FoldoutGroup("Run Second")]
        public float Second;

        public override float GetDuration()
        {
            return Second;
        }

        protected override void OnFeedbackPlay()
        {
            
        }

        protected override void OnFeedbackStop()
        {
            
        }
    }
}
