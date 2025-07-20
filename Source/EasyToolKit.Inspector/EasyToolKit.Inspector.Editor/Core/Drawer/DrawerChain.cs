using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

namespace EasyToolKit.Inspector.Editor
{
    public class DrawerChain : IEnumerable<IEasyDrawer>, IEnumerator<IEasyDrawer>
    {
        private readonly IEasyDrawer[] _drawers;
        private int _index = -1;

        public IEasyDrawer Current
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

        public IEasyDrawer[] Drawers => _drawers;

        public InspectorProperty Property { get; private set; }

        public DrawerChain(InspectorProperty property, IEnumerable<IEasyDrawer> drawers)
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


        public IEnumerator<IEasyDrawer> GetEnumerator()
        {
            return ((IEnumerable<IEasyDrawer>)_drawers).GetEnumerator();
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
