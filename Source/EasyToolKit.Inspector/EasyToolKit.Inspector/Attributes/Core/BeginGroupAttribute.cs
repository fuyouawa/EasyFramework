using System;

namespace EasyToolKit.Inspector
{
    public abstract class BeginGroupAttribute : Attribute
    {
        public string GroupName;
        public bool EndAfterThisProperty;

        protected BeginGroupAttribute()
        {
        }
    }
}
