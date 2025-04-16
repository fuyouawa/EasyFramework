using System;

namespace EasyFramework.ToolKit
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AddFeedbackMenuAttribute : Attribute
    {
        public string Path { get; }

        public AddFeedbackMenuAttribute(string path)
        {
            Path = path;
        }
    }
}
