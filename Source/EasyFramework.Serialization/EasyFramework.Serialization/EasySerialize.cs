using System.Collections.Generic;
using UnityEngine;

namespace EasyFramework.Serialization
{
    public partial class EasySerialize
    {
        internal enum Format
        {
            Binary,
            Json,
            Xml,
            Yaml
        }

        internal byte[] To<T>(T value, Format format, List<Object> referencedUnityObjects)
        {
            var ios = Native.AllocStringIoStream();

        }
    }
}
