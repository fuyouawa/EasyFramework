using System;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    [AddEasyFeedbackMenu("游戏物体/调用函数")]
    public class EF_GameObjectInvokeMethod : AbstractEasyFeedback
    {
        public MethodPicker Picker = new MethodPicker();
        
        public override string Tip => "调用GameObject上的一个函数";

        protected override void OnFeedbackPlay()
        {
            try
            {
                Picker.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e, Owner.gameObject);
            }
        }

        protected override void OnFeedbackStop()
        {
        }
    }
}
