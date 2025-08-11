using System;

namespace EasyToolKit.Inspector
{
    public class ValueDropdownAttribute : Attribute
    {
        public string OptionsGetter;

        public ValueDropdownAttribute(string optionsGetter)
        {
            OptionsGetter = optionsGetter;
        }
    }
}