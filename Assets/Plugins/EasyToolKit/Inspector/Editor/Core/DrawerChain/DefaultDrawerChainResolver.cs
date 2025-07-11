namespace EasyToolKit.Inspector.Editor
{
    public class DefaultDrawerChainResolver : DrawerChainResolver
    {
        public static readonly DefaultDrawerChainResolver Instance = new DefaultDrawerChainResolver();

        public override DrawerChain GetDrawerChain(InspectorProperty property)
        {
            throw new System.NotImplementedException();
        }
    }
}
