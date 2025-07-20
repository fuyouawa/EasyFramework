using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class DefaultDrawerChainResolver : DrawerChainResolver
    {
        private DrawerChain _chain;

        public override DrawerChain GetDrawerChain()
        {
            if (_chain != null)
            {
                return _chain;
            }

            var drawerTypeResults = DrawerUtility.GetDefaultPropertyDrawerTypes(Property);
            var drawers = new List<IEasyDrawer>();
            foreach (var drawerType in drawerTypeResults.Select(result => result.MatchedType).Distinct())
            {
                var drawer = drawerType.CreateInstance<IEasyDrawer>();
                drawer.Property = Property;
                drawer.Initialize();
                drawers.Add(drawer);
            }

            _chain = new DrawerChain(Property, drawers);
            return _chain;
        }
    }
}
