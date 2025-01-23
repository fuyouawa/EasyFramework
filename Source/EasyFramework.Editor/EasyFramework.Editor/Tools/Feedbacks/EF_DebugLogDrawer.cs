using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using System;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_DebugLogDrawer : AbstractEasyFeedbackDrawer<EF_DebugLog>
    {
        protected override void PostBuildPropertiesGroups()
        {
            PropertiesGroups.Add(new PropertiesGroupInfo("调试打印设置", rect =>
            {
                Feedback.Duration = EasyEditorField.Value(
                    EditorHelper.TempContent("持续时间"),
                    Feedback.Duration);
                Feedback.Message = EasyEditorField.Value(
                    EditorHelper.TempContent("打印信息"),
                    Feedback.Message);
                    
                Feedback.LogOnInit = EasyEditorField.Value(
                    EditorHelper.TempContent("在初始化时打印"),
                    Feedback.LogOnInit);

                if (Feedback.LogOnInit)
                {
                    Feedback.MessageOnInit = EasyEditorField.Value(
                        EditorHelper.TempContent("初始化时的打印信息"),
                        Feedback.MessageOnInit);
                }
                    
                Feedback.LogOnStop = EasyEditorField.Value(
                    EditorHelper.TempContent("在停止时打印"),
                    Feedback.LogOnStop);

                if (Feedback.LogOnStop)
                {
                    Feedback.MessageOnStop = EasyEditorField.Value(
                        EditorHelper.TempContent("结束时的打印信息"),
                        Feedback.MessageOnStop);
                }

                Feedback.LogOnReset = EasyEditorField.Value(
                    EditorHelper.TempContent("在结束时打印"),
                    Feedback.LogOnReset);

                if (Feedback.LogOnReset)
                {
                    Feedback.MessageOnReset = EasyEditorField.Value(
                        EditorHelper.TempContent("重置时的打印信息"),
                        Feedback.MessageOnReset);
                }
            }));
        }
    }
}
