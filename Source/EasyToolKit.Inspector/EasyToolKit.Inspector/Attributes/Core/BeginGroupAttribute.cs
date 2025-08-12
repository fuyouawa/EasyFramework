using System;

namespace EasyToolKit.Inspector
{
    public abstract class BeginGroupAttribute : Attribute
    {
        public string GroupName { get; set; }
        public bool EndAfterThisProperty { get; set; }

        protected BeginGroupAttribute()
        {
        }
    }
}
