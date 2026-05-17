using UnityEngine;
using UnityEngine.UI;

public class MeasCrosshairs : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] private float _size = 64f;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private bool _enableCrosshairs = true;
    private Image _image;

    public void Draw(Canvas canvas)
    {
        if (!_enableCrosshairs)
        {
            return;
        }

        GameObject crosshairObj = new GameObject("MeasuringTool:MeasCrosshairs");
        crosshairObj.transform.SetParent(canvas.transform, false);

        _image = crosshairObj.AddComponent<Image>();
        _image.sprite = _sprite;

        RectTransform rect = crosshairObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(_size, _size);
    }
}
