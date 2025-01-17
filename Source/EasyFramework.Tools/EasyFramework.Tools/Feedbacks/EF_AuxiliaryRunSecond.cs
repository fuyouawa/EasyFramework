using Sirenix.OdinInspector;

namespace EasyGameFramework
{
    [AddEasyFeedbackMenu("辅助/运行指定时间(秒)")]
    public class EF_AuxiliaryRunSecond : AbstractEasyFeedback
    {
        [FoldoutGroup("Run Second")]
        public float Second;
        
        public override string Tip => "运行指定时间(秒)";

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
