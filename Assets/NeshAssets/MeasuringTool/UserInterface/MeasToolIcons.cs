using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeasToolIcons : MonoBehaviour
{
    [SerializeField, UnityEngine.Range(0f, 100f)] private float _size = 64;
    [SerializeField, UnityEngine.Range(-100f, 100f)] private float _horizontalPosition = -40f;
    [SerializeField, UnityEngine.Range(-100f, 100f)] private float _verticalPosition = 40f;

    Image _image;

    public void Draw(Canvas canvas)
    {
        GameObject iconObject = new GameObject("MeasuringTools:MeasToolIcons");
        iconObject.transform.SetParent(canvas.transform, false);

        _image = iconObject.AddComponent<Image>();

        RectTransform rect = iconObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(_horizontalPosition, _verticalPosition);
        rect.sizeDelta = new Vector2(_size, _size);
    }

    public void UpdateIcon(Sprite icon)
    {
        _image.sprite = icon;
    }

    private List<GameObject> _toolBeltObjects = new List<GameObject>(3);
    private List<Sprite> _toolIcons = new List<Sprite>();
    private int _activeToolIdx = -1;
    private bool _toolBeltInit = false;

    public void DrawToolBelt(Canvas canvas, List<IMeasTool> _tools)
    {
        if (!_toolBeltInit) InitialiseToolBelt(_tools);
    }

    private void InitialiseToolBelt(List<IMeasTool> _tools)
    {
        foreach (IMeasTool tool in _tools)
        {
            _toolIcons.Add(tool.ToolIcon);
        }
        _activeToolIdx = 0;
        _toolBeltInit = true;
    }

    private GameObject CreateToolIcon(Sprite toolIcon)
    {
        GameObject iconObject = new GameObject("MeasuringTools:MeasToolIcons");
        iconObject.transform.SetParent(transform, false);

        _image = iconObject.AddComponent<Image>();
        _image.sprite = toolIcon;

        RectTransform rect = iconObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(_size, _size);

        return iconObject;
    }

}
