using System;

namespace EasyFramework.ToolKit
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GameConsoleCommandAttribute : Attribute
    {
        public string Name { get; }
        public bool OptionalParameter { get; set; } = false;
        public string Description { get; set; }

        public GameConsoleCommandAttribute(string name)
        {
            Name = name.Trim();
        }
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public class GameConsoleCommandExampleAttribute : Attribute
    {
        public string Name { get; }

        public GameConsoleCommandExampleAttribute(string name)
        {
            Name = name;
        }
    }
}
