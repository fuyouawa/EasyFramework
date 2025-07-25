using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.Core.Editor
{
    //TODO 版权问题
    public class GUIScopeStack<T> : Stack<T>
    {
        private GUIFrameCounter guiState = new GUIFrameCounter();

        public new void Push(T t)
        {
            if (this.guiState.Update().IsNewFrame)
            {
                this.Clear();
            }

            base.Push(t);
        }

        public new T Pop()
        {
            if (this.Count == 0 || this.guiState.Update().IsNewFrame)
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
