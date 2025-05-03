using System;

namespace EasyFramework.ToolKit
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GameConsoleCommandAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsSystem { get; set; }
        public bool OptionalParameter { get; set; } = false;

        public GameConsoleCommandAttribute(string name, string description)
        {
            Name = name.Trim();
            Description = description.Trim();
        }

        public GameConsoleCommandAttribute(string name)
            : this(name, "")
        {
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
