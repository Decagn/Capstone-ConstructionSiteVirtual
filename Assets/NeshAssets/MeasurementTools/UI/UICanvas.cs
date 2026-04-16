using UnityEngine;

public class UICanvas : MonoBehaviour
{
    [SerializeField] MeasurementToolHandler _toolHandler;
    [SerializeField] Crosshairs _crosshairs;
    [SerializeField] MeasurementText _text;
    [SerializeField] ToolIcon _icon;

    private GameObject _canvasObj;
    private Canvas _canvas;

    private void Awake()
    {
        SubscribeToToolHandler();
        CreateCanvas();

        _crosshairs.Draw(_canvas);
        _text.Draw(_canvas);
        _icon.Draw(_canvas);
    }
    private void OnDisable()
    {
        UnsubscribedFromToolHandler();
    }

    private void CreateCanvas()
    {
        _canvasObj = new GameObject("MeasurementTools:UICanvas");
        _canvas = _canvasObj.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

    private void HandleNewMeasurement(float newMeasurement, IMeasurementTool tool)
    {
        _text.UpdateText(newMeasurement, tool);
    }
    private void SwitchTool(IMeasurementTool tool)
    {
        _text.ClearText(tool);
        _icon.UpdateIcon(tool);
    }
    private void HandleSelectedPointsReset(IMeasurementTool tool)
    {
        _text.ClearText(tool);
    }

    private void SubscribeToToolHandler()
    {
        _toolHandler.OnMeasurementUpdated += HandleNewMeasurement;
        _toolHandler.OnSwitchTool += SwitchTool;
        _toolHandler.OnSelectedPointsResettedManually += HandleSelectedPointsReset;
        _toolHandler.OnDeselectLastPoint += HandleSelectedPointsReset;
    }
    private void UnsubscribedFromToolHandler()
    {
        _toolHandler.OnMeasurementUpdated -= HandleNewMeasurement;
        _toolHandler.OnSwitchTool -= SwitchTool;
        _toolHandler.OnSelectedPointsResettedManually -= HandleSelectedPointsReset;
        _toolHandler.OnDeselectLastPoint -= HandleSelectedPointsReset;
    }
}