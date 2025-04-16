using System;
using System.Collections;

namespace EasyFramework.ToolKit
{
    public interface IFeedback
    {
        string Label { get; }
        string Tip { get; }
        bool Enable { get; set; }
        bool IsPlaying { get; }

        void Reset();
        IEnumerator PlayCo();
        void Stop();

        void OnEnable();
        void OnDisable();
        void OnDestroy();
        void Setup(Feedbacks owner);
        void Initialize();
    }
}
