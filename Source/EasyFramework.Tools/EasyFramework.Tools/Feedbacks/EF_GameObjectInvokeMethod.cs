using EasyFramework.Utilities;

namespace EasyFramework.Tools
{
    [AddEasyFeedbackMenu("游戏物体/调用函数")]
    public class EF_GameObjectInvokeMethod : AbstractEasyFeedback
    {
        public MethodPicker Picker = new MethodPicker();
        
        public override string Tip => "调用GameObject上的一个函数";

        protected override void OnFeedbackPlay()
        {
            Picker.TryInvoke(out _);
        }

        protected override void OnFeedbackStop()
        {
        }
    }
}
