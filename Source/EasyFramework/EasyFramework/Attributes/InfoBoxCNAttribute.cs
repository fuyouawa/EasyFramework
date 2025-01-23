using System;
using System.Diagnostics;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework
{
    /// <summary>
    /// 扩展自Sirenix.OdinInspector.InfoBoxAttribute
    /// 优化中文字体的显示
    /// 使用方法和Sirenix.OdinInspector.InfoBoxAttribute一样
    /// </summary>
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public class InfoBoxCNAttribute : PropertyAttribute
    {
        /// <summary>
        /// The message to display in the info box.
        /// </summary>
        public string Message;

        /// <summary>
        /// The type of the message box.
        /// </summary>
        public InfoMessageType InfoMessageType;

        /// <summary>
        /// Optional member field, property or function to show and hide the info box.
        /// </summary>
        public string VisibleIf;

        /// <summary>
        /// When <c>true</c> the InfoBox will ignore the GUI.enable flag and always draw as enabled.
        /// </summary>
        public bool GUIAlwaysEnabled;

        /// <summary>Supports a variety of color formats, including named colors (e.g. "red", "orange", "green", "blue"), hex codes (e.g. "#FF0000" and "#FF0000FF"), and RGBA (e.g. "RGBA(1,1,1,1)") or RGB (e.g. "RGB(1,1,1)"), including Odin attribute expressions (e.g "@this.MyColor"). Here are the available named colors: black, blue, clear, cyan, gray, green, grey, magenta, orange, purple, red, transparent, transparentBlack, transparentWhite, white, yellow, lightblue, lightcyan, lightgray, lightgreen, lightgrey, lightmagenta, lightorange, lightpurple, lightred, lightyellow, darkblue, darkcyan, darkgray, darkgreen, darkgrey, darkmagenta, darkorange, darkpurple, darkred, darkyellow.</summary>
        public string IconColor;

        private SdfIconType icon;

        /// <summary>
        /// The icon to be displayed next to the message.
        /// </summary>
        public SdfIconType Icon
        {
            get
            {
                return icon;
            }
            set
            {
                icon = value;
                HasDefinedIcon = true;
            }
        }

        public bool HasDefinedIcon { get; private set; }

        /// <summary>
        /// Displays an info box above the property.
        /// </summary>
        /// <param name="message">The message for the message box. Supports referencing a member string field, property or method by using $.</param>
        /// <param name="infoMessageType">The type of the message box.</param>
        /// <param name="visibleIfMemberName">Name of member bool to show or hide the message box.</param>
        public InfoBoxCNAttribute(string message, InfoMessageType infoMessageType = InfoMessageType.Info, string visibleIfMemberName = null)
        {
            Message = message;
            InfoMessageType = infoMessageType;
            VisibleIf = visibleIfMemberName;
        }

        /// <summary>
        /// Displays an info box above the property.
        /// </summary>
        /// <param name="message">The message for the message box. Supports referencing a member string field, property or method by using $.</param>
        /// <param name="visibleIfMemberName">Name of member bool to show or hide the message box.</param>
        public InfoBoxCNAttribute(string message, string visibleIfMemberName)
        {
            Message = message;
            InfoMessageType = InfoMessageType.Info;
            VisibleIf = visibleIfMemberName;
        }

        /// <summary>
        /// Displays an info box above the property.
        /// </summary>
        /// <param name="message">The message for the message box. Supports referencing a member string field, property or method by using $.</param>
        /// <param name="icon">The icon to be displayed next to the message.</param>
        /// <param name="visibleIfMemberName">Name of member bool to show or hide the message box.</param>
        public InfoBoxCNAttribute(string message, SdfIconType icon, string visibleIfMemberName = null)
        {
            Message = message;
            Icon = icon;
            VisibleIf = visibleIfMemberName;
            InfoMessageType = InfoMessageType.None;
        }
    }
}
