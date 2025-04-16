using System;
using System.Collections;

namespace EasyFramework.ToolKit
{
    public class AddFeedbackMenuAttribute : Attribute
    {
        public string Path { get; }

        public AddFeedbackMenuAttribute(string path)
        {
            Path = path;
        }
    }

    public interface IFeedback
    {
        string Label { get; }
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
