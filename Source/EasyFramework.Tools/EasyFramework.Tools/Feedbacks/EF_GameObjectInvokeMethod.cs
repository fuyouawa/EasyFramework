using Sirenix.OdinInspector;

namespace EasyGameFramework
{
    [AddEasyFeedbackMenu("游戏物体/调用函数")]
    public class EF_GameObjectInvokeMethod : AbstractEasyFeedback
    {
        [Required]
        [FoldoutGroup("Invoke Method")]
        [HideReferenceObjectPicker]
        [HideLabel]
        [DisableInPlayMode]
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
