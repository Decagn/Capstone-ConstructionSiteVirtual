using UnityEngine;
using UnityEngine.UI;

/*
 * Measurement Crosshairs
 * This class creates the crosshairs for the measurement tools.
 */
public class MeasurementCrosshairs : MonoBehaviour
{
    // Attach the crosshairs sprite here in the editor.
    [SerializeField] private Sprite _crosshairsSprite;
    [SerializeField] private bool _enableCrosshairs = true;
    [SerializeField, Range(0f, 100f)] private float _crosshairsSize = 64f;

    private Image _image;

    public void Draw(Canvas canvas)
    {
        if (!_enableCrosshairs)
            return;

        GameObject crosshairObj = new GameObject("Measurement Crosshairs");
        crosshairObj.transform.SetParent(canvas.transform, false);

        _image = crosshairObj.AddComponent<Image>();
        _image.sprite = _crosshairsSprite;

        RectTransform rect = crosshairObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(_crosshairsSize, _crosshairsSize);
    }
}
