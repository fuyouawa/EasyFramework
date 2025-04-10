using UnityEngine.Events;

namespace EasyFramework
{
    public static class UnityEventExtension
    {
        public static IFromRegister AddListenerEx(this UnityEvent unityEvent, UnityAction action)
        {
            unityEvent.AddListener(action);
            return new FromRegisterGeneric(() => unityEvent.RemoveListener(action));
        }
    }
}
