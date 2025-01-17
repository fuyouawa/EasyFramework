using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.Tools
{
    [AddEasyFeedbackMenu("游戏物体/设置激活")]
    public class EF_GameObjectSetActive : AbstractEasyFeedback
    {
        [FoldoutGroup("Set Active")]
        public GameObject BoundObject;
        [FoldoutGroup("Set Active")]
        public bool ActiveToSet = true;
        
        public override string Tip => "设置GameObject激活状态";

        protected override void OnFeedbackPlay()
        {
            BoundObject.SetActive(ActiveToSet);
        }

        protected override void OnFeedbackStop()
        {
        }
    }
}
