using System;
using System.Diagnostics;

namespace EasyToolKit.Inspector
{
    public enum InlineEditorStyle
    {
        Place,
        Box,
        Foldout,
    }

    [Conditional("UNITY_EDITOR")]
    public class InlineEditorAttribute : Attribute
    {
        public InlineEditorStyle Style;
    }
}
