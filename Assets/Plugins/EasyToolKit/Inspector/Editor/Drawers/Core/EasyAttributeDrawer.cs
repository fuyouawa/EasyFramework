using System;

namespace EasyToolKit.Inspector.Editor
{
    public class EasyAttributeDrawer<TAttribute> : EasyDrawer
        where TAttribute : Attribute
    {
        private TAttribute _attribute;

        public TAttribute Attribute
        {
            get
            {
                if (_attribute == null)
                {
                    //TODO
                }
                return _attribute;
            }
        }
    }
}
