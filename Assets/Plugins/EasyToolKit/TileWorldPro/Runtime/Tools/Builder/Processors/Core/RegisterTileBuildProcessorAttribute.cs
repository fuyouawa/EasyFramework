using System;

namespace EasyToolKit.TileWorldPro
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterTileBuildProcessorAttribute : Attribute
    {
        public Type ProcessorType { get; }
        public string Name { get; }

        public RegisterTileBuildProcessorAttribute(Type processorType, string name)
        {
            ProcessorType = processorType;
            Name = name;
        }
    }
}