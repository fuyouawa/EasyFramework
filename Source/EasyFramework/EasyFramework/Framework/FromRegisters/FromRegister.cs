using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyFramework
{
    public interface IFromRegister : IUnregister
    {
    }

    public class FromRegisterGeneric : UnregisterGeneric, IFromRegister
    {
        public FromRegisterGeneric(Action onUnregister) : base(onUnregister)
        {
        }
    }

    public abstract class UnregisterTrigger : MonoBehaviour
    {
        private readonly HashSet<IUnregister> _unregisters = new HashSet<IUnregister>();

        public void AddUnregister(IUnregister unregister) => _unregisters.Add(unregister);

        public void RemoveUnregister(IUnregister unregister) => _unregisters.Remove(unregister);

        public void Unregister()
        {
            foreach (var unregister in _unregisters)
            {
                unregister.Unregister();
            }

            _unregisters.Clear();
        }
    }

    public class UnregisterOnDestroyTrigger : UnregisterTrigger
    {
        private void OnDestroy()
        {
            Unregister();
        }
    }

    public class UnregisterOnDisableTrigger : UnregisterTrigger
    {
        private void OnDestroy()
        {
            Unregister();
        }

        private void OnDisable()
        {
            Unregister();
        }
    }

    public static class IFromRegisterExtension
    {
        public static IUnregister UnregisterWhenDestroyed(
            this IFromRegister unRegister,
            GameObject gameObject)
        {
            var trigger = gameObject.GetOrAddComponent<UnregisterOnDestroyTrigger>();
            trigger.AddUnregister(unRegister);
            return unRegister;
        }

        public static IUnregister UnregisterWhenDisabled(
            this IFromRegister unRegister,
            GameObject gameObject)
        {
            var trigger = gameObject.GetOrAddComponent<UnregisterOnDisableTrigger>();
            trigger.AddUnregister(unRegister);
            return unRegister;
        }
    }
}
