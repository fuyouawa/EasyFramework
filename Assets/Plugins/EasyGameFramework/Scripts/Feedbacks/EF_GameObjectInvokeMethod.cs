using Sirenix.OdinInspector;

namespace EasyGameFramework
{
    [EasyFeedbackHelper("调用GameObject上的一个函数")]
    [AddEasyFeedbackMenu("游戏物体/调用函数")]
    public class EF_GameObjectInvokeMethod : AbstractEasyFeedback
    {
        [Required]
        [FoldoutGroup("Invoke Method")]
        [HideReferenceObjectPicker]
        [HideLabel]
        [DisableInPlayMode]
        public MethodPicker Picker = new();
        
        protected override void OnFeedbackPlay()
        {
            Picker.TryInvoke(out _);
        }

        protected override void OnFeedbackStop()
        {
        }
    }
}
