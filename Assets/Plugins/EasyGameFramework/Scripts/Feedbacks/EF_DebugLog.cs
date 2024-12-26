using EasyFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyGameFramework
{
    [EasyFeedbackHelper("打印调试信息")]
    [AddEasyFeedbackMenu("调试/打印")]
    public class EF_DebugLog : AbstractEasyFeedback
    {
        [FoldoutGroup("打印")]
        [LabelText("持续时间")]
        public float Duration;

        [FoldoutGroup("打印")]
        [LabelText("不打印如果空信息")]
        public bool NotLogIfEmpty = true;
        [FoldoutGroup("打印")]
        [LabelText("打印信息")]
        public string Message = "HelloWorld!";
        [FoldoutGroup("打印")]
        [LabelText("初始化时的打印信息")]
        public string MessageOnInit = "OnInit";
        [FoldoutGroup("打印")]
        [LabelText("结束时的打印信息")]
        public string MessageOnStop = "OnStop";
        [FoldoutGroup("打印")]
        [LabelText("重置时的打印信息")]
        public string MessageOnReset = "OnReset";

        public override float GetDuration()
        {
            return Duration;
        }

        protected override void OnFeedbackInit()
        {
            if (NotLogIfEmpty && MessageOnInit.IsNullOrEmpty())
                return;
            Debug.Log(MessageOnInit);
        }

        protected override void OnFeedbackPlay()
        {
            if (NotLogIfEmpty && Message.IsNullOrEmpty())
                return;
            Debug.Log(Message);
        }

        protected override void OnFeedbackStop()
        {
            if (NotLogIfEmpty && MessageOnStop.IsNullOrEmpty())
                return;
            Debug.Log(MessageOnStop);
        }

        protected override void OnFeedbackReset()
        {
            if (NotLogIfEmpty && MessageOnReset.IsNullOrEmpty())
                return;
            Debug.Log(MessageOnReset);
        }
    }
}
