using System;

namespace EasyFramework.ToolKit
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GameConsoleCommandAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; set; }
        internal bool IsSystem { get; set; }

        public GameConsoleCommandAttribute(string name)
        {
            Name = name.Trim();
        }
    }
}
