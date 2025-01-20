using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_DebugLogDrawer : AbstractEasyFeedbackDrawer<EF_DebugLog>
    {
        private InspectorProperty _duration;
        private InspectorProperty _message;
        private InspectorProperty _logOnInit;
        private InspectorProperty _messageOnInit;
        private InspectorProperty _logOnStop;
        private InspectorProperty _messageOnStop;
        private InspectorProperty _logOnReset;
        private InspectorProperty _messageOnReset;

        protected override void Initialize()
        {
            base.Initialize();
            _duration = Property.Children["Duration"];
            _message = Property.Children["Message"];
            _logOnInit = Property.Children["LogOnInit"];
            _messageOnInit = Property.Children["MessageOnInit"];
            _logOnStop = Property.Children["LogOnStop"];
            _messageOnStop = Property.Children["MessageOnStop"];
            _logOnReset = Property.Children["LogOnReset"];
            _messageOnReset = Property.Children["MessageOnReset"];
        }

        protected override void DrawOtherPropertyLayout()
        {
            var value = ValueEntry.SmartValue;

            _duration.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_duration, this),
                "调试打印设置", _duration.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    _duration.Draw(new GUIContent("持续时间"));
                    _message.Draw(new GUIContent("打印信息"));

                    _logOnInit.Draw(new GUIContent("在初始化时打印"));
                    if (value.LogOnInit)
                    {
                        _messageOnInit.Draw(new GUIContent("初始化时的打印信息"));
                    }

                    _logOnStop.Draw(new GUIContent("在停止时打印"));
                    if (value.LogOnStop)
                    {
                        _messageOnStop.Draw(new GUIContent("结束时的打印信息"));
                    }

                    _logOnReset.Draw(new GUIContent("在结束时打印"));
                    if (value.LogOnReset)
                    {
                        _messageOnReset.Draw(new GUIContent("重置时的打印信息"));
                    }
                }
            });
        }
    }
}
