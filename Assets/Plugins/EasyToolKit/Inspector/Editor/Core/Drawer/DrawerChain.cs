using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class DrawerChain : IEnumerator<EasyDrawer>, IEnumerable<EasyDrawer>
    {
        public abstract EasyDrawer Current { get; }
        public abstract bool MoveNext();
        public abstract void Reset();

        object IEnumerator.Current => Current;

        public IEnumerator<EasyDrawer> GetEnumerator()
        {
            return this;
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
