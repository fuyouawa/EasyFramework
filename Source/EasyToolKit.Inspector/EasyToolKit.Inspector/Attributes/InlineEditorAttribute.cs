using System;
using System.Diagnostics;

namespace EasyToolKit.Inspector
{
    public enum InlineEditorStyle
    {
        Place,
        PlaceWithHide,
        Box,
        Foldout,
        FoldoutBox,
    }

    [Conditional("UNITY_EDITOR")]
    public class InlineEditorAttribute : Attribute
    {
        public InlineEditorStyle Style { get; set; }
    }
}
