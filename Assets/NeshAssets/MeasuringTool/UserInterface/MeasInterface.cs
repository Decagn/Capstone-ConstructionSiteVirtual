using Unity.VisualScripting;
using UnityEngine;

public class MeasInterface : MonoBehaviour
{
    #region Unity Lifecycle Events
    private void Awake()
    {
        CreateCanvas();

        DrawToolIcons();
        ReadToolSwitch();

        DrawCrosshairs();
    }
    private void OnDisable()
    {
        StopReadingToolSwitch();
    }
    #endregion

    #region UICanvas
    private GameObject _canvasObj;
    private Canvas _canvas;

    private void CreateCanvas()
    {
        _canvasObj = new GameObject("MeasInterface:UICanvas");
        _canvasObj.transform.SetParent(transform, false);
        _canvas = _canvasObj.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }
    #endregion

    #region Tool Icon 
    [SerializeField] MeasManager _manager;
    [SerializeField] MeasToolIcons _icons;

    private void DrawToolIcons()
    {
        _icons.Draw(_canvas);
    }
    private void UpdateToolIcon()
    {
        //if (!_iconsDrawn) DrawToolIcons();
        _icons.UpdateIcon(_manager.GetActiveToolIcon());
    }
    private void ReadToolSwitch()
    {
        _manager.OnToolSwitch += UpdateToolIcon;
    }
    private void StopReadingToolSwitch()
    {
        _manager.OnToolSwitch -= UpdateToolIcon;
    }
    #endregion

    #region Crosshairs
    [SerializeField] MeasCrosshairs _crosshairs;

    private void DrawCrosshairs()
    {
        _crosshairs.Draw(_canvas);
    }
    #endregion
}
