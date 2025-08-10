namespace EasyToolKit.Inspector.Editor
{
    public interface IPropertyValueEntryWrapper : IPropertyValueEntry
    {
    }

    public interface IPropertyValueEntryWrapper<TValue, TBaseValue> : IPropertyValueEntryWrapper, IPropertyValueEntry<TValue>
        where TValue : TBaseValue
    {
    }
}