using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EasyFramework.Core
{
    public class UniTaskSequence
    {
        class Clip
        {
            private readonly List<UniTask> _tasks = new List<UniTask>();
            private Action _callback;

            public void AddTask(UniTask task)
            {
                _tasks.Add(task);
            }

            public void AddCallback(Action callback)
            {
                _callback += callback;
            }

            public UniTask GetTask()
            {
                _callback?.Invoke();
                return UniTask.WhenAll(_tasks);
            }
        }

        private readonly List<Clip> _clips = new List<Clip>();

        public UniTaskSequence Append(UniTask task)
        {
            var clip = new Clip();
            clip.AddTask(task);
            _clips.Add(clip);
            return this;
        }

        public UniTaskSequence AppendCallback(Action callback)
        {
            var clip = new Clip();
            clip.AddCallback(callback);
            _clips.Add(clip);
            return this;
        }

        public UniTaskSequence Join(UniTask task)
        {
            if (_clips.Count == 0)
            {
                return Append(task);
            }

            var clip = _clips.Last();
            clip.AddTask(task);
            return this;
        }

        public UniTaskSequence JoinCallback(Action callback)
        {
            if (_clips.Count == 0)
            {
                return AppendCallback(callback);
            }

            var clip = _clips.Last();
            clip.AddCallback(callback);
            return this;
        }

        public async UniTask GetTask()
        {
            foreach (var clip in _clips)
            {
                await clip.GetTask();
            }
        }
    }
}
