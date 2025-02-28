using System.Collections.Generic;
using System;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public class ViewBinderGameObjectCheckAttribute : PropertyAttribute
    {
    }

    [Serializable]
    public class OtherViewBinderTarget
    {
        public GameObject Target;
    }
    
    [Serializable]
    public class OtherViewBinderTargets
    {
        [ViewBinderGameObjectCheck]
        public List<GameObject> Collection;
    }

    public sealed class OtherViewBinders : MonoBehaviour
    {
        public OtherViewBinderTargets Targets;
    }
}
