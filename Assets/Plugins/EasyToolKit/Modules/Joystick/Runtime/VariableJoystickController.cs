using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EasyToolKit.Joystick
{
    public class VariableJoystickController : JoystickController
    {
        public float MoveThreshold
        {
            get { return moveThreshold; }
            set { moveThreshold = Mathf.Abs(value); }
        }

        [SerializeField] private float moveThreshold = 1;
        [SerializeField] private JoystickType joystickType = JoystickType.Fixed;

        private Vector2 fixedPosition = Vector2.zero;

        public void SetMode(JoystickType joystickType)
        {
            this.joystickType = joystickType;
            if (joystickType == JoystickType.Fixed)
            {
                Background.anchoredPosition = fixedPosition;
                Background.gameObject.SetActive(true);
            }
            else
                Background.gameObject.SetActive(false);
        }

        protected override void Start()
        {
            base.Start();
            fixedPosition = Background.anchoredPosition;
            SetMode(joystickType);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (joystickType != JoystickType.Fixed)
            {
                Background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
                Background.gameObject.SetActive(true);
            }

            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (joystickType != JoystickType.Fixed)
                Background.gameObject.SetActive(false);

            base.OnPointerUp(eventData);
        }

        protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
        {
            if (joystickType == JoystickType.Dynamic && magnitude > moveThreshold)
            {
                Vector2 difference = normalised * (magnitude - moveThreshold) * radius;
                Background.anchoredPosition += difference;
            }

            base.HandleInput(magnitude, normalised, radius, cam);
        }
    }

    public enum JoystickType
    {
        Fixed,
        Floating,
        Dynamic
    }
}
