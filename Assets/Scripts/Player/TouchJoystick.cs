using UnityEngine;
using UnityEngine.EventSystems;

public class TouchJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Joystick Settings")]
    public float joystickRange = 50f;

    private RectTransform _background;
    private RectTransform _handle;
    private Vector2 _input = Vector2.zero;

    public Vector2 Input => _input;

    void Awake()
    {
        _background = GetComponent<RectTransform>();
        _handle = transform.GetChild(0).GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _background, eventData.position, eventData.pressEventCamera, out pos);

        pos = Vector2.ClampMagnitude(pos, joystickRange);
        _handle.localPosition = pos;
        _input = pos / joystickRange;
    }

    public void OnPointerUpHandler(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData)
    {
        _handle.localPosition = Vector2.zero;
        _input = Vector2.zero;
    }
}