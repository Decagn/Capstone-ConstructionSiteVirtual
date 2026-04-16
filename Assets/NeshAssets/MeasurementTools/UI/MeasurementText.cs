using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MeasurementText : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] private float _fontSize = 20;
    [SerializeField, Range(-100f, 100f)] private float _horizontalPosition = 0f;
    [SerializeField, Range(-100f, 100f)] private float _verticalPosition = -75f;
    private TextMeshProUGUI _text;

    public void Draw(Canvas canvas)
    {
        GameObject textObject = new GameObject("MeasurementTools:MeasurementText");
        textObject.transform.SetParent(canvas.transform, false);

        _text = textObject.AddComponent<TextMeshProUGUI>();
        _text.fontSize = _fontSize;
        _text.alignment = TextAlignmentOptions.Center;
        _text.color = Color.white;
        _text.text = "";

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(_horizontalPosition, _verticalPosition);
        rect.sizeDelta = new Vector2(200f, 30f);
    }

    public void UpdateText(float newMeasurement, IMeasurementTool tool)
    {
        if (tool is TapeMeasure) _text.text = $"{newMeasurement:F2}m";
        else if (tool is Protractor) _text.text = $"{newMeasurement:F1}°";
    }

    public void ClearText(IMeasurementTool tool)
    {
        if (tool is TapeMeasure) _text.text = "select two points";
        else if (tool is Protractor) _text.text = "select three points";
    }
}
