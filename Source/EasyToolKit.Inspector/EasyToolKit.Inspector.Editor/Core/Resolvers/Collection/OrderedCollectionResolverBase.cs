namespace EasyToolKit.Inspector.Editor
{
    public interface IOrderedCollectionResolver : ICollectionResolver
    {
        void QueueInsertElementAt(int index, object value);
        void QueueRemoveElementAt(int index);
        void QueueMoveElemenetAt(int sourceIndex, int destinationIndex);
    }

    public abstract class OrderedCollectionResolverBase : CollectionResolverBase, IOrderedCollectionResolver
    {
        public void QueueInsertElementAt(int index, object value)
        {
            EnqueueChange(() => InsertElementAt(index, value));
        }

        public void QueueRemoveElementAt(int index)
        {
            EnqueueChange(() => RemoveElementAt(index));
        }

        public void QueueMoveElemenetAt(int sourceIndex, int destinationIndex)
        {
            EnqueueChange(() => MoveElemenetAt(sourceIndex, destinationIndex));
        }

        protected abstract void InsertElementAt(int index, object value);

        protected abstract void RemoveElementAt(int index);

        protected abstract void MoveElemenetAt(int sourceIndex, int destinationIndex);
    }
}
