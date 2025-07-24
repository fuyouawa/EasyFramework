using EasyToolKit.Inspector;
using System.Diagnostics;

[assembly: RegisterGroupAttributeScope(typeof(AwesomeBoxGroupAttribute), typeof(EndAwesomeBoxGroupAttribute))]

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    public class AwesomeBoxGroupAttribute : BeginGroupAttribute
    {
        public string Label;
        public string IconTextureGetter;

        public AwesomeBoxGroupAttribute(string label)
        {
            Label = label;
        }
    }

    public class EndAwesomeBoxGroupAttribute : EndGroupAttribute
    {
    }
}
