using UnityEngine;

namespace EasyFramework.ToolKit
{
    [AddFeedbackMenu("游戏物体/设置启用状态")]
    public class GameObjectSetActiveFeedback : AbstractFeedback
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
