using System;

namespace EasyFramework.Serialization
{
    public interface IArchive : IDisposable
    {
        void SetNextName(string name);
        void StartNode();
        void FinishNode();

        bool Process(ref int value);
        bool Process(ref float value);
        bool Process(ref double value);
        bool Process(ref string str);
        bool Process(ref byte[] data);
        bool Process(ref UnityEngine.Object unityObject);
    }
}
