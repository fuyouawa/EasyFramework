namespace EasyToolKit.Inspector.Editor
{
    public interface IOrderedCollectionResolver : ICollectionResolver
    {
        void QueueInsertElementAt(int index, object value);
        void QueueRemoveElementAt(int index);
        void QueueMoveElemenetAt(int sourceIndex, int destinationIndex);
    }

    public abstract class OrderedCollectionResolverBase<TElement> : CollectionResolverBase<TElement>, IOrderedCollectionResolver
    {
        public void QueueInsertElementAt(int index, object value)
        {
            EnqueueChange(() => InsertElementAt(index, (TElement)value));
        }

        public void QueueRemoveElementAt(int index)
        {
            EnqueueChange(() => RemoveElementAt(index));
        }

        public void QueueMoveElemenetAt(int sourceIndex, int destinationIndex)
        {
            EnqueueChange(() => MoveElemenetAt(sourceIndex, destinationIndex));
        }

        protected abstract void InsertElementAt(int index, TElement value);

        protected abstract void RemoveElementAt(int index);

        protected abstract void MoveElemenetAt(int sourceIndex, int destinationIndex);
    }
}
