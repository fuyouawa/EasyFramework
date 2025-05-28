using UnityEngine;

namespace EasyFramework.ToolKit
{
    public enum HideMode
    {
        OnAwake,
        OnEnable,
        OnStart,
    }

    public class Hide : MonoBehaviour
    {
        public HideMode Mode;

        void Awake()
        {
            if (Mode == HideMode.OnAwake)
            {
                gameObject.SetActive(false);
            }
        }

        void OnEnable()
        {
            if (Mode == HideMode.OnEnable)
            {
                gameObject.SetActive(false);
            }
        }

        void Start()
        {
            if (Mode == HideMode.OnStart)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
