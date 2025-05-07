using System.Collections;
using EasyFramework.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EasyFramework.ToolKit
{
    public class GameConsoleLogPopupDragger : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler,
        IEndDragHandler
    {
        [SerializeField] private RectTransform _target;

        [SerializeField] private Vector2 _snapPadding = Vector2.one;
        [SerializeField] private float _snapDuration = 0.3f;
        private Canvas _canvas;

        private bool _isDraging;
        protected Coroutine _smoothMoveCo;

        void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (!_isDraging)
            {
                GameConsole.Instance.ShowLogWindow();
            }
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            _isDraging = true;
            if (_smoothMoveCo != null)
                StopCoroutine(_smoothMoveCo);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvas.GetComponent<RectTransform>(),
                    eventData.position, eventData.pressEventCamera,
                    out var localPoint))
            {
                _target.anchoredPosition = localPoint;
            }
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            _isDraging = false;

            Vector2 localPos = _target.anchoredPosition;
            Vector2 canvasSize = _canvas.GetComponent<RectTransform>().sizeDelta;

            // 计算四个方向到边的距离
            float left = Mathf.Abs(localPos.x - (-canvasSize.x * 0.5f));
            float right = Mathf.Abs(localPos.x - (canvasSize.x * 0.5f));
            float top = Mathf.Abs(localPos.y - (canvasSize.y * 0.5f));
            float bottom = Mathf.Abs(localPos.y - (-canvasSize.y * 0.5f));

            // 找最近的边
            float min = Mathf.Min(left, right, top, bottom);
            var targetPos = localPos;

            var paddingX = _snapPadding.x + _target.sizeDelta.x * 0.5f;
            var paddingY = _snapPadding.y + _target.sizeDelta.y * 0.5f;

            if (min.Approximately(left))
            {
                targetPos.x = -canvasSize.x * 0.5f + paddingX;
            }
            else if (min.Approximately(top))
            {
                targetPos.y = canvasSize.y * 0.5f - paddingX;
            }
            else if (min.Approximately(bottom))
            {
                targetPos.y = -canvasSize.y * 0.5f + paddingY;
            }
            else // if (min.Approximately(right))
            {
                targetPos.x = canvasSize.x * 0.5f - paddingY;
            }

            if (_smoothMoveCo != null)
                StopCoroutine(_smoothMoveCo);
            _smoothMoveCo = StartCoroutine(SmoothMoveTo(targetPos, _snapDuration));
        }

        private IEnumerator SmoothMoveTo(Vector2 target, float duration)
        {
            Vector2 start = _target.anchoredPosition;
            float time = 0;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                t = Mathf.SmoothStep(0, 1, t); // 更平滑

                _target.anchoredPosition = Vector2.Lerp(start, target, t);
                yield return null;
            }

            _target.anchoredPosition = target;
        }
    }
}
