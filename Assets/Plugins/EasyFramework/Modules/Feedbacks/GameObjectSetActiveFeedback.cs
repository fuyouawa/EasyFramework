using EasyFramework.ToolKit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.Modules
{
    [AddFeedbackMenu("游戏物体/设置激活状态")]
    public class GameObjectSetActiveFeedback : AbstractFeedback
    {
        [FoldoutGroup("激活设置")]
        [LabelText("游戏对象")]
        public GameObject Target;

        [FoldoutGroup("激活设置")]
        [LabelText("激活设置")]
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
