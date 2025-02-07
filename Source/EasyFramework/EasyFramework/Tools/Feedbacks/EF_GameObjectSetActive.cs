using UnityEngine;

namespace EasyFramework.Feedbacks
{
    [AddEasyFeedbackMenu("游戏物体/设置启用状态")]
    public class EF_GameObjectSetActive : AbstractEasyFeedback
    {
        public GameObject Target;
        public bool ActiveToSet = true;
        
        public override string Tip => "设置游戏对象的启用状态";

        protected override void OnFeedbackPlay()
        {
            Target.SetActive(ActiveToSet);
        }

        protected override void OnFeedbackStop()
        {
        }
    }
}
