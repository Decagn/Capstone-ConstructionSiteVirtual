using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolCrosshairsVisualiser : MonoBehaviour
{
    private ToolManager _toolManager;

    private GameObject _canvasGameObject;
    private Canvas _canvas;

    private Image _crosshairsImage;
    private Sprite _crosshairsSprite;

    private TextMeshProUGUI _measurementText;

    private Image _toolIconImage;
    private Sprite _toolIconSprite;

    public void Initialise(ToolManager toolManager, Sprite crosshairsSprite)
    {
        ConnectToolManager(toolManager);
        toolManager.activeTool.OnSelectedPointsUpdated += UpdateMeasurementText;

        CreateCanvas();

        _crosshairsSprite = crosshairsSprite;
        DrawCrosshairs();

        DrawMeasurementText();

        _toolIconSprite = _toolManager.activeTool.ToolIcon;
        DrawToolIcon();
    }

    private void CreateCanvas()
    {
        _canvasGameObject = new GameObject("MeasurementTools:CrosshairCanvas");
        _canvas = _canvasGameObject.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

    private void DrawCrosshairs()
    {
        GameObject crosshairObject = new GameObject("NeasurementTools:Crosshairs");
        crosshairObject.transform.SetParent(_canvas.transform, false);

        _crosshairsImage = crosshairObject.AddComponent<Image>();
        _crosshairsImage.sprite = _crosshairsSprite;

        RectTransform rect = crosshairObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(64f, 64f);
    }

    private void DrawMeasurementText()
    {
        GameObject textObject = new GameObject("MeasurementTools:MeasurementText");
        textObject.transform.SetParent(_canvas.transform, false);

        _measurementText = textObject.AddComponent<TextMeshProUGUI>();
        _measurementText.fontSize = 20;
        _measurementText.alignment = TextAlignmentOptions.Center;
        _measurementText.color = Color.white;
        _measurementText.text = "";

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, -75f);
        rect.sizeDelta = new Vector2(200f, 30f);
    }

    private void DrawToolIcon()
    {
        GameObject iconObject = new GameObject("MeasurementTools:ToolIcon");
        iconObject.transform.SetParent(_canvas.transform, false);

        _toolIconImage = iconObject.AddComponent<Image>();
        _toolIconImage.sprite = _toolIconSprite;

        RectTransform rect = iconObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(-40f, 40f);
        rect.sizeDelta = new Vector2(64f, 64f);
    }

    public void UpdateMeasurementText()
    {
        bool HasLengths = _toolManager.activeTool.MeasuredLengths.Count > 0;
        bool HasAngles = _toolManager.activeTool.MeasuredAngles.Count > 0;

        if (HasLengths)
        {
            float newLength = _toolManager.activeTool.MeasuredLengths[0];
            _measurementText.text = $"{newLength:F2}m";
        }

        if (HasAngles)
        {
            float newAngle = _toolManager.activeTool.MeasuredAngles[0];
            _measurementText.text = $"{newAngle:F1}°";
        }
    }

    private void ConnectToolManager(ToolManager manager)
    {
        _toolManager = manager;
    }
}
