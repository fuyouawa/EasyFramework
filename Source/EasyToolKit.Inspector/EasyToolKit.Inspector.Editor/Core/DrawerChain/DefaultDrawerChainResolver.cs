using System.Collections.Generic;
using System.Linq;

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
            var drawers = new List<EasyDrawer>();
            foreach (var drawerType in drawerTypeResults.Select(result => result.MatchedType).Distinct())
            {
                drawers.Add(EasyDrawer.Create(drawerType, Property));
            }

            _chain = new DrawerChain(Property, drawers);
            return _chain;
        }
    }
}
