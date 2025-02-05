using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyFramework
{
    public interface IFromRegister : IUnRegister
    {
    }

    public class FromRegister : CustomUnRegister, IFromRegister
    {
        public FromRegister(Action onUnRegister) : base(onUnRegister)
        {
        }
    }

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

    public static class IFromRegisterExtension
    {
        public static IUnRegister UnRegisterWhenDestroyed(
            this IFromRegister unRegister,
            GameObject gameObject)
        {
            var trigger = gameObject.GetOrAddComponent<UnRegisterOnDestroyTrigger>();
            trigger.AddUnRegister(unRegister);
            return unRegister;
        }

        public static IUnRegister UnRegisterWhenDisabled(
            this IFromRegister unRegister,
            GameObject gameObject)
        {
            var trigger = gameObject.GetOrAddComponent<UnRegisterOnDisableTrigger>();
            trigger.AddUnRegister(unRegister);
            return unRegister;
        }
    }
}
