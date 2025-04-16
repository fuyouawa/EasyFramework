using System;
using EasyFramework.ToolKit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.Modules
{
    [AddFeedbackMenu("游戏物体/调用函数")]
    public class GameObjectInvokeMethodFeedback : AbstractFeedback
    {
        [FoldoutGroup("函数调用")]
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
