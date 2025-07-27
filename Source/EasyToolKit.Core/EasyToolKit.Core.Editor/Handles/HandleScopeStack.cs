using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.Core.Editor
{
    public class HandleScopeStack<T> : Stack<T>
    {
        public new void Push(T t)
        {
            base.Push(t);
        }

        public new T Pop()
        {
            if (this.Count == 0)
            {
                Debug.LogError("Pop call mismatch; no corresponding push call! Each call to Pop must always correspond to one - and only one - call to Push.");
                return default(T);
            }
            else
            {
                return base.Pop();
            }
        }
    }
}
