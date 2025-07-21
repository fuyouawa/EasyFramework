using System;
using System.Diagnostics;
using UnityEngine;

namespace EasyToolKit.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [Conditional("UNITY_EDITOR")]
    public class AssetsOnlyAttribute : Attribute
    {
        
    }
}
