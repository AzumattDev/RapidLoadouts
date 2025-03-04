using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RapidLoadouts.UI
{
    public class UIDragger : EventTrigger
    {
        public delegate void UIDroppedHandler(object source, Vector3 newLocalPosition);

        public event UIDroppedHandler OnUIDropped = null!;

        private bool _isDragging;
        private Vector2 _offset;
        public RectTransform rectTransform { get; private set; } = null!;

        public void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void Update()
        {
            if (!_isDragging) return;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, Input.mousePosition, null, out Vector2 localPoint))
            {
                rectTransform.anchoredPosition = localPoint - _offset;
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right || !Input.GetKey(KeyCode.LeftControl)) return;
            _isDragging = true;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, Input.mousePosition, null, out Vector2 localPoint))
            {
                _offset = localPoint - rectTransform.anchoredPosition;
            }
        }

        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right) return;
            _isDragging = false;
            OnUIDropped?.Invoke(this, new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y, -1f));
        }
    }
}