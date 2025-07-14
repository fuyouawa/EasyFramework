namespace EasyToolKit.Inspector.Editor
{
    public abstract class InspectorAttributeAccessorsResolver
    {
        public abstract IAttributeAccessor[] GetAttributeAccessors(InspectorProperty property);
    }
}
