using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThrottleStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Settings")]
    [SerializeField] private float _range = 100f;
    [SerializeField] private RectTransform _handle;

    // √аз Ч от 0 до 1, не пружинит
    public float Throttle { get; private set; } = 0f;
    // Yaw Ч от -1 до 1, пружинит
    public float Yaw { get; private set; } = 0f;

    private RectTransform _rectTransform;
    private int _pointerId = -1;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        Throttle = 0f;
        Yaw = 0f;

        if (_handle != null)
            _handle.localPosition = new Vector3(0f, -_range, 0f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pointerId = eventData.pointerId;
        UpdateFromPointer(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateFromPointer(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pointerId = -1;

        // Yaw пружинит обратно в ноль
        Yaw = 0f;

        // Throttle остаЄтс€ на месте Ч не сбрасываем

        // ќбновл€ем визуал Ч Handle по X возвращаетс€ в центр
        if (_handle != null)
            _handle.localPosition = new Vector3(0f, _handle.localPosition.y, 0f);
    }

    private void UpdateFromPointer(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform,
            screenPosition,
            null,
            out Vector2 localPoint
        );

        // ¬ертикаль Ч Throttle, не пружинит
        float clampedY = Mathf.Clamp(localPoint.y, -_range, _range);
        Throttle = (clampedY + _range) / (_range * 2f);

        // √оризонталь Ч Yaw, от -1 до 1
        float clampedX = Mathf.Clamp(localPoint.x, -_range, _range);
        Yaw = clampedX / _range;

        // ќбновл€ем визуал
        if (_handle != null)
            _handle.localPosition = new Vector3(clampedX, clampedY, 0f);
    }
}
