using System;
using System.Collections.Generic;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    public interface ICollectionResolver
    {
        Type ElementType { get; }

        void QueueInsertElement(object value);
        void QueueRemoveElement(object value);
    }
    
    public abstract class CollectionResolverBase<TElement> : PropertyResolver, ICollectionResolver
    {
        private Action _changeAction;

        public Type ElementType => typeof(TElement);

        public void QueueInsertElement(object value)
        {
            EnqueueChange(() => InsertElement((TElement)value));
        }

        public void QueueRemoveElement(object value)
        {
            EnqueueChange(() => RemoveElement((TElement)value));
        }

        protected abstract void InsertElement(TElement value);
        protected abstract void RemoveElement(TElement value);
        
        protected void EnqueueChange(Action action)
        {
            _changeAction += action;
            Property.Tree.QueueCallbackUntilRepaint(() =>
            {
                Property.Tree.SetPropertyDirty(Property);
            });
        }

        protected override bool ApplyChanges()
        {
            if (_changeAction != null)
            {
                foreach (var target in Property.Tree.Targets)
                {
                    Undo.RecordObject(target, $"Change {Property.Info.PropertyPath} on {target.name}");
                }

                _changeAction();
                _changeAction = null;

                Property.Update();
                foreach (var child in Property.Children.Recurse())
                {
                    child.Update();
                }

                Property.Children.Refresh();
                return true;
            }

            return false;
        }
    }
}
