using System;

namespace EasyToolKit.Inspector.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DrawerPriorityAttribute : Attribute
    {
        public DrawerPriority Priority { get; }

        public DrawerPriorityAttribute(DrawerPriorityLevel level)
        {
            Priority = new DrawerPriority(level);
        }

        public DrawerPriorityAttribute(int value = 0)
        {
            Priority = new DrawerPriority(value);
        }
    }
}
