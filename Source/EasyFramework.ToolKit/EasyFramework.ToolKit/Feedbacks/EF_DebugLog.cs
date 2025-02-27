using UnityEngine;

namespace EasyFramework.ToolKit
{
    [AddEasyFeedbackMenu("调试/打印")]
    public class EF_DebugLog : AbstractEasyFeedback
    {
        public float Duration;
        public string Message = "HelloWorld!";
        public bool LogOnInit = false;
        public string MessageOnInit = "OnInit";
        public bool LogOnStop = false;
        public string MessageOnStop = "OnStop";
        public bool LogOnReset = false;
        public string MessageOnReset = "OnReset";
        
        public override string Tip => "打印调试信息";

        public override float GetDuration()
        {
            return Duration;
        }

        protected override void OnFeedbackInit()
        {
            if (LogOnInit)
            {
                Debug.Log(MessageOnInit);
            }
        }

        protected override void OnFeedbackPlay()
        {
            Debug.Log(Message);
        }

        protected override void OnFeedbackStop()
        {
            if (LogOnStop)
            {
                Debug.Log(MessageOnStop);
            }
        }

        protected override void OnFeedbackReset()
        {
            if (LogOnReset)
            {
                Debug.Log(MessageOnReset);
            }
        }
    }
}
