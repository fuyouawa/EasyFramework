using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EasyToolKit.Inspector.Editor
{
    public class DrawerChain : IReadOnlyList<EasyDrawer>
    {
        private readonly List<EasyDrawer> _chain;

        public InspectorProperty Property { get; private set; }
        public int Count => _chain.Count;
        public EasyDrawer this[int index] => _chain[index];

        public DrawerChain(InspectorProperty property, IEnumerable<EasyDrawer> chain)
        {
            Property = property;
            _chain = chain.ToList();
        }

        public IEnumerator<EasyDrawer> GetEnumerator()
        {
            return _chain.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
