using System;
using System.Diagnostics;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UniTool.Attributes
{
    /// <summary>
    /// 扩展自Sirenix.OdinInspector.InfoBoxAttribute
    /// 优化中文字体的显示
    /// 使用方法和Sirenix.OdinInspector.InfoBoxAttribute一样
    /// </summary>
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public class InformationAttribute : PropertyAttribute
    {
        public string Message;

        public InfoMessageType MessageType;

        public string VisibleIf;

        public bool GUIAlwaysEnabled;

        public string IconColor;

        public InformationAttribute(string message)
        {
            Message = message;
            MessageType = InfoMessageType.Info;
        }

        public InformationAttribute(string message, InfoMessageType messageType, string visiableIf = null)
        {
            Message = message;
            MessageType = messageType;
            VisibleIf = visiableIf;
        }
    }
}
