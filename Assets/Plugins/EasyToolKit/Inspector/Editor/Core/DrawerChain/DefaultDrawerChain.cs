using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

namespace EasyToolKit.Inspector.Editor
{
    public class DefaultDrawerChain : DrawerChain
    {
        private readonly EasyDrawer[] _drawerChain;
        private int _index = -1;

        public  override EasyDrawer Current
        {
            get
            {
                if (_index >0 && _index < _drawerChain.Length)
                {
                    return _drawerChain[_index];
                }
                return null;
            }
        }

        public InspectorProperty Property { get; private set; }

        public DefaultDrawerChain(InspectorProperty property, IEnumerable<EasyDrawer> chain)
        {
            Property = property;
            _drawerChain = chain.ToArray();
        }

        public override bool MoveNext()
        {
            do
            {
                _index++;
            } while (Current != null && this.Current.SkipWhenDrawing);

            return Current != null;
        }

        public  override void Reset()
        {
            _index = -1;
        }
    }
}
