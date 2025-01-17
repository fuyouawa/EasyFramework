using System;
using System.Collections;
using UnityEngine;

namespace EasyFramework.Utilities
{
    public static class MonoBehaviourExtension
    {
        public static void CallInNextFrame(this MonoBehaviour mono, Action callback)
        {
            mono.StartCoroutine(CallInNextFrameCo(callback));
        }

        private static IEnumerator CallInNextFrameCo(Action callback)
        {
            yield return null;
            callback();
        }
    }
}
