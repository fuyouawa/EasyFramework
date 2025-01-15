using System;
using EasyFramework;
using UnityEngine;

namespace EasyGameFramework
{
    [Serializable]
    public struct SerializableType
    {
        [SerializeField] private string _assemblyQualifiedName;
        private Type _typeCache;

        public Type Type
        {
            get
            {
                if (_assemblyQualifiedName.IsNullOrEmpty())
                    return null;

                if (_typeCache == null)
                {
                    _typeCache = Type.GetType(_assemblyQualifiedName, true);
                }

                Debug.Assert(_typeCache.AssemblyQualifiedName == _assemblyQualifiedName);

                return _typeCache;
            }
            set
            {
                _assemblyQualifiedName = value.AssemblyQualifiedName;
                _typeCache = null;
            }
        }
    }
}
