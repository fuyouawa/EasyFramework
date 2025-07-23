using System;

namespace EasyToolKit.Inspector
{
    public abstract class BeginGroupAttribute : Attribute
    {
        public string GroupName;

        protected BeginGroupAttribute()
        {
        }
    }
}
