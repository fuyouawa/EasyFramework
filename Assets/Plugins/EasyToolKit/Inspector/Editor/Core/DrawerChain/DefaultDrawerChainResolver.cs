using System;
using EasyToolKit.ThirdParty.OdinSerializer.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace EasyToolKit.Inspector.Editor
{
    public class DefaultDrawerChainResolver : DrawerChainResolver
    {
        public static readonly DefaultDrawerChainResolver Instance = new DefaultDrawerChainResolver();
        
        private static readonly Dictionary<Type, Func<EasyDrawer>> FastDrawerCreators = new Dictionary<Type, Func<EasyDrawer>>(FastTypeComparer.Instance);

        public override DrawerChain GetDrawerChain(InspectorProperty property)
        {
            var drawerTypeResults = DrawerUtility.GetDefaultPropertyDrawerTypes(property);
            var drawers = new List<EasyDrawer>();
            foreach (var drawerType in drawerTypeResults.Select(result => result.MatchedType).Distinct())
            {
                drawers.Add(CreateDrawer(drawerType));
            }

            return new DrawerChain(property, drawers);
        }

        private static EasyDrawer CreateDrawer(Type drawerType)
        {
            if (!FastDrawerCreators.TryGetValue(drawerType, out var fastCreator))
            {
                var constructor = drawerType.GetConstructor(Type.EmptyTypes);
                var method = new DynamicMethod(drawerType.FullName + "_FastCreator", typeof(EasyDrawer), Type.EmptyTypes);

                var il = method.GetILGenerator();

                il.Emit(OpCodes.Newobj, constructor);
                il.Emit(OpCodes.Ret);

                fastCreator = (Func<EasyDrawer>)method.CreateDelegate(typeof(Func<EasyDrawer>));
                FastDrawerCreators.Add(drawerType, fastCreator);
            }

            return fastCreator();
        }
    }
}
