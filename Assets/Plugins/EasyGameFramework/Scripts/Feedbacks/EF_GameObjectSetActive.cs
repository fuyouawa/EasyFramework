using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyGameFramework
{
    [EasyFeedbackHelper("设置GameObject激活状态")]
    [AddEasyFeedbackMenu("游戏物体/设置激活")]
    public class EF_GameObjectSetActive : AbstractEasyFeedback
    {
        [FoldoutGroup("Set Active")]
        public GameObject BoundObject;
        [FoldoutGroup("Set Active")]
        public bool ActiveToSet = true;

        protected override void OnFeedbackPlay()
        {
            BoundObject.SetActive(ActiveToSet);
        }

        protected override void OnFeedbackStop()
        {
        }
    }
}
