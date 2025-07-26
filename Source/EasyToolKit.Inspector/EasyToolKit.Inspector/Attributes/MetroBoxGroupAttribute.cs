using EasyToolKit.Inspector;
using System.Diagnostics;

[assembly: RegisterGroupAttributeScope(typeof(MetroBoxGroupAttribute), typeof(EndMetroBoxGroupAttribute))]

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    public class MetroBoxGroupAttribute : BeginGroupAttribute
    {
        public string Label;
        public string IconTextureGetter;

        public MetroBoxGroupAttribute(string label)
        {
            Label = label;
        }
    }

    public class EndMetroBoxGroupAttribute : EndGroupAttribute
    {
    }
}
