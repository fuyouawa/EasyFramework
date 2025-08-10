namespace EasyToolKit.Inspector.Editor
{
    public interface IPropertyValueCollectionWrapper : IPropertyValueCollection
    {
    }

    public interface IPropertyValueCollectionWrapper<TValue, TBaseValue> : IPropertyValueCollectionWrapper, IPropertyValueCollection<TValue>
        where TValue : TBaseValue
    {
    }
}