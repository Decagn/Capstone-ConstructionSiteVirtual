using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Crosshairs : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] private float _size = 64f;
    [SerializeField] private Sprite _sprite;
    private Image _image;

    public void Draw(Canvas canvas)
    {
        GameObject crosshairObj = new GameObject("MeasurementTools:Crosshairs");
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
