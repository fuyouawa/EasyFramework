using System.Collections.Generic;
using System;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    [Serializable]
    public class OtherViewBinderTarget
    {
        public GameObject Target;
        public Component Binder;
    }
    
    [Serializable]
    public class OtherViewBinderTargets
    {
        public List<OtherViewBinderTarget> Collection;
    }

    public sealed class OtherViewBinders : MonoBehaviour
    {
        public OtherViewBinderTargets Targets;
    }
}
