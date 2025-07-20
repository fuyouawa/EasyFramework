namespace EasyToolKit.Inspector.Editor
{
    public interface IOrderedCollectionResolver : ICollectionResolver
    {
        void InsertElementAt(int index, object value);
        void RemoveElementAt(int index);
        void MoveElemenetAt(int sourceIndex, int destinationIndex);
    }

    public abstract class OrderedCollectionResolverBase : CollectionResolverBase, IOrderedCollectionResolver
    {
        public abstract void InsertElementAt(int index, object value);

        public abstract void RemoveElementAt(int index);

        public abstract void MoveElemenetAt(int sourceIndex, int destinationIndex);
    }
}
