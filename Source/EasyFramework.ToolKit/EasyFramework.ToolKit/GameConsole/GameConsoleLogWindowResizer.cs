using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EasyFramework.ToolKit
{
    public class GameConsoleLogWindowResizer : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        [SerializeField] private RectTransform _target;
        [SerializeField] private float _minimumHeight = 240f;
        [SerializeField] private float _maximumHeight = 480f;
        
        private Vector2 _startMousePos;
        private float _startHeight;

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            // 把鼠标屏幕坐标转换为面板的本地坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _target, eventData.position, eventData.pressEventCamera, out _startMousePos);
            
            // 记录当前面板的高度
            _startHeight = _target.rect.height;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            // 实时获取当前鼠标相对于面板的局部坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _target, eventData.position, eventData.pressEventCamera, out var currentMousePos);

            // 计算鼠标 Y 方向的移动差值
            float deltaY = currentMousePos.y - _startMousePos.y;
            // 面板的新高度（限制在Min和Max范围内）
            float newHeight = Mathf.Clamp(_startHeight - deltaY, _minimumHeight, _maximumHeight);

            // 更新目标面板的高度
            _target.sizeDelta = _target.sizeDelta.NewY(newHeight);
        }
    }
}
