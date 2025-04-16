using EasyFramework.ToolKit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.Modules
{
    [AddFeedbackMenu("调试/打印")]
    public class DebugLogFeedback : AbstractFeedback
    {
        [FoldoutGroup("调试打印设置")]
        [LabelText("打印信息")]
        public string Message = "HelloWorld!";

        [FoldoutGroup("调试打印设置")]
        [LabelText("在初始化时打印")]
        public bool LogOnInit = false;

        [FoldoutGroup("调试打印设置")]
        [ShowIf(nameof(LogOnInit))]
        [LabelText("初始化时的打印信息")]
        public string MessageOnInit = "OnInit";

        [FoldoutGroup("调试打印设置")]
        [LabelText("在停止时打印")]
        public bool LogOnStop = false;

        [FoldoutGroup("调试打印设置")]
        [ShowIf(nameof(LogOnStop))]
        [LabelText("停止时的打印信息")]
        public string MessageOnStop = "OnStop";

        [FoldoutGroup("调试打印设置")]
        [LabelText("在重置时打印")]
        public bool LogOnReset = false;

        [FoldoutGroup("调试打印设置")]
        [ShowIf(nameof(LogOnReset))]
        [LabelText("重置时的打印信息")]
        public string MessageOnReset = "OnReset";
        
        public override string Tip => "打印调试信息";

        protected override void OnFeedbackInit()
        {
            if (LogOnInit)
            {
                Debug.Log(MessageOnInit, Owner.gameObject);
            }
        }

        protected override void OnFeedbackPlay()
        {
            Debug.Log(Message, Owner.gameObject);
        }

        protected override void OnFeedbackStop()
        {
            if (LogOnStop)
            {
                Debug.Log(MessageOnStop, Owner.gameObject);
            }
        }

        protected override void OnFeedbackReset()
        {
            if (LogOnReset)
            {
                Debug.Log(MessageOnReset, Owner.gameObject);
            }
        }
    }
}
