using System;

namespace EasyToolKit.Inspector
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterGroupAttributeScopeAttribute : Attribute
    {
        public Type BeginGroupAttributeType { get; set; }
        public Type EndGroupAttributeType { get; set; }

        public RegisterGroupAttributeScopeAttribute(Type beginGroupAttributeType, Type endGroupAttributeType)
        {
            if (!typeof(BeginGroupAttribute).IsAssignableFrom(beginGroupAttributeType))
            {
                throw new ArgumentException($"{beginGroupAttributeType.Name} must inherit from BeginGroupAttribute");
            }

            if (!typeof(EndGroupAttribute).IsAssignableFrom(endGroupAttributeType))
            {
                throw new ArgumentException($"{endGroupAttributeType.Name} must inherit from EndGroupAttribute");
            }

            BeginGroupAttributeType = beginGroupAttributeType;
            EndGroupAttributeType = endGroupAttributeType;
        }
    }
}
