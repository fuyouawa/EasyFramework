using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

namespace EasyToolKit.Inspector.Editor
{
    public class DrawerChain : IEnumerable<EasyDrawer>, IEnumerator<EasyDrawer>
    {
        private readonly EasyDrawer[] _drawers;
        private int _index = -1;

        public EasyDrawer Current
        {
            get
            {
                if (_index >= 0 && _index < _drawers.Length)
                {
                    return _drawers[_index];
                }
                return null;
            }
        }

        public EasyDrawer[] Drawers => _drawers;

        public InspectorProperty Property { get; private set; }

        public DrawerChain(InspectorProperty property, IEnumerable<EasyDrawer> drawers)
        {
            Property = property;
            _drawers = drawers.ToArray();
        }

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            do
            {
                _index++;
            } while (Current != null && this.Current.SkipWhenDrawing);

            return Current != null;
        }

        public void Reset()
        {
            _index = -1;
        }


        public IEnumerator<EasyDrawer> GetEnumerator()
        {
            return ((IEnumerable<EasyDrawer>)_drawers).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void IDisposable.Dispose()
        {
            Reset();
        }
    }
}
