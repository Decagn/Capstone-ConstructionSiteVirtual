using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsTooltip : MonoBehaviour
{
    [SerializeField] private int _fontSize = 24;
    [SerializeField] private Color _textColour = Color.white;
    [SerializeField] private Color _backgroundColour = new Color32(99, 117, 103, 255);

    private Canvas _canvas;

    public void Initialise(string controls, string keys)
    {
        BuildCanvas();
        BuildPanel(controls, keys);
    }

    private void BuildCanvas()
    {
        GameObject canvasObject = new GameObject("ControlsTooltip:Canvas");
        _canvas = canvasObject.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 5;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
    }

    private void BuildPanel(string controls, string keys)
    {
        GameObject panel = new GameObject("ControlsTooltip:Panel");
        panel.transform.SetParent(_canvas.transform, false);

        Image background = panel.AddComponent<Image>();
        background.color = _backgroundColour;

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-20f, -20f);
        rect.sizeDelta = new Vector2(400f, 375f);

        BuildKeysColumn(panel, keys);
        BuildControlsColumn(panel, controls);
    }

    // Column of keys (Q, E, W, A, S, D, etc.).
    private void BuildKeysColumn(GameObject panel, string text)
    {
        GameObject columnObject = new GameObject("ControlsTooltip:KeysColumn");
        columnObject.transform.SetParent(panel.transform, false);

        TextMeshProUGUI columnText = columnObject.AddComponent<TextMeshProUGUI>();
        columnText.text = text;
        columnText.fontSize = _fontSize;
        columnText.color = _textColour;
        columnText.alignment = TextAlignmentOptions.MidlineRight;

        RectTransform columnRect = columnObject.GetComponent<RectTransform>();
        columnRect.anchorMin = new Vector2(0f, 0f);
        columnRect.anchorMax = new Vector2(0.45f, 1f);
        columnRect.offsetMin = new Vector2(10f, 10f);
        columnRect.offsetMax = new Vector2(-5f, -10f);
    }

    // Column of controls (Fly Up, Fly Down, Walk, etc.).
    private void BuildControlsColumn(GameObject panel, string text)
    {
        GameObject columnObject = new GameObject("ControlsTooltip:ControlsColumn");
        columnObject.transform.SetParent(panel.transform, false);

        TextMeshProUGUI columnText = columnObject.AddComponent<TextMeshProUGUI>();
        columnText.text = text;
        columnText.fontSize = _fontSize;
        columnText.color = _textColour;
        columnText.alignment = TextAlignmentOptions.MidlineLeft;

        RectTransform columnRect = columnObject.GetComponent<RectTransform>();
        columnRect.anchorMin = new Vector2(0.45f, 0f);
        columnRect.anchorMax = new Vector2(1f, 1f);
        columnRect.offsetMin = new Vector2(5f, 10f);
        columnRect.offsetMax = new Vector2(-10f, -10f);
    }
}