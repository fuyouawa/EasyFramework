using System.Collections.Generic;
using UnityEngine;

namespace EasyFramework
{
    public abstract class UnRegisterTrigger : MonoBehaviour
    {
        private readonly HashSet<IUnRegisterConfiguration> _unRegisters = new HashSet<IUnRegisterConfiguration>();

        public void AddUnRegister(IUnRegisterConfiguration unRegister) => _unRegisters.Add(unRegister);

        public void RemoveUnRegister(IUnRegisterConfiguration unRegister) => _unRegisters.Remove(unRegister);

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
        public static IUnRegisterConfiguration UnRegisterWhenDestroyed(
            this IUnRegisterConfiguration unRegister,
            GameObject gameObject)
        {
            var trigger = gameObject.GetOrAddComponent<UnRegisterOnDestroyTrigger>();
            trigger.AddUnRegister(unRegister);
            return unRegister;
        }

        public static IUnRegisterConfiguration UnRegisterWhenDisabled(
            this IUnRegisterConfiguration unRegister,
            GameObject gameObject)
        {
            var trigger = gameObject.GetOrAddComponent<UnRegisterOnDisableTrigger>();
            trigger.AddUnRegister(unRegister);
            return unRegister;
        }
    }
}
