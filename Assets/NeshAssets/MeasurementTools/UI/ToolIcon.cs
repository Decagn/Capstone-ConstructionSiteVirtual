using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ToolIcon : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] private float _size = 64;
    [SerializeField, Range(-100f, 100f)] private float _horizontalPosition = -40f;
    [SerializeField, Range(-100f, 100f)] private float _verticalPosition = 40f;

    Image _image;

    public void Draw(Canvas canvas)
    {
        GameObject iconObject = new GameObject("MeasurementTools:ToolIcon");
        iconObject.transform.SetParent(canvas.transform, false);

        _image = iconObject.AddComponent<Image>();

        RectTransform rect = iconObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(_horizontalPosition, _verticalPosition);
        rect.sizeDelta = new Vector2(_size, _size);
    }

    public void UpdateIcon(IMeasurementTool tool)
    {
        _image.sprite = tool.ToolIcon;
    }
}
