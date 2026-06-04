using UnityEngine;
using UnityEngine.UI;

/*
 * Measurement Tool Icon
 * This class creates and displays the current tool icon.
 * The tool icon sprites are attached to the tool objects.
 */
public class MeasurementToolIcon : MonoBehaviour
{
    [SerializeField, UnityEngine.Range(0f, 100f)] private float _size = 64;
    [SerializeField, UnityEngine.Range(-100f, 100f)] private float _horizontalPosition = -40f;
    [SerializeField, UnityEngine.Range(-100f, 100f)] private float _verticalPosition = 40f;

    // Set this in the editor to the icon of the first tool in the tool list.
    // This fixes a bug where the icon does not load when the scene first starts up.
    [SerializeField] private Sprite _defaultToolIcon;

    Image _image;

    public void Draw(Canvas canvas)
    {
        GameObject iconObject = new GameObject("Measurement Tool Icon");
        iconObject.transform.SetParent(canvas.transform, false);

        _image = iconObject.AddComponent<Image>();
        _image.sprite = _defaultToolIcon;

        RectTransform rect = iconObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(_horizontalPosition, _verticalPosition);
        rect.sizeDelta = new Vector2(_size, _size);
    }

    // Called when the tool is switched to a different one.
    public void UpdateIcon(Sprite icon) { _image.sprite = icon; }
}
