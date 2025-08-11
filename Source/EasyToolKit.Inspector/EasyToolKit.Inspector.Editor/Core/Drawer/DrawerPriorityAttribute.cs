using System;

namespace EasyToolKit.Inspector.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DrawerPriorityAttribute : Attribute
    {
        public DrawerPriority Priority { get; }

        public DrawerPriorityAttribute(double value = DrawerPriorityLevel.Default)
        {
            Priority = new DrawerPriority(value);
        }
    }
}
