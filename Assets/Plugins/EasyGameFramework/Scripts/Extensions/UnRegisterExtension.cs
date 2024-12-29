using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyFramework;
using UnityEngine;

namespace EasyGameFramework
{
    public abstract class UnRegisterTrigger : MonoBehaviour
    {
        private readonly HashSet<IUnRegister> _unRegisters = new HashSet<IUnRegister>();

        public void AddUnRegister(IUnRegister unRegister) => _unRegisters.Add(unRegister);

        public void RemoveUnRegister(IUnRegister unRegister) => _unRegisters.Remove(unRegister);

        public void UnRegister()
        {
            foreach (var unRegister in _unRegisters)
            {
                unRegister.UnRegister();
            }

            _unRegisters.Clear();
        }
    }

    public class UnRegisterOnDestroyTrigger : UnRegisterTrigger
    {
        private void OnDestroy()
        {
            UnRegister();
        }
    }

    public class UnRegisterOnDisableTrigger : UnRegisterTrigger
    {
        private void OnDestroy()
        {
            UnRegister();
        }

        private void OnDisable()
        {
            UnRegister();
        }
    }

    public static class UnRegisterExtension
    {
        public static IUnRegister UnRegisterWhenDestroyed(
            this IUnRegister unRegister,
            GameObject gameObject)
        {
            var trigger = gameObject.GetOrAddComponent<UnRegisterOnDestroyTrigger>();
            trigger.AddUnRegister(unRegister);
            return unRegister;
        }

        public static IUnRegister UnRegisterWhenDisabled(
            this IUnRegister unRegister,
            GameObject gameObject)
        {
            var trigger = gameObject.GetOrAddComponent<UnRegisterOnDisableTrigger>();
            trigger.AddUnRegister(unRegister);
            return unRegister;
        }
    }
}
