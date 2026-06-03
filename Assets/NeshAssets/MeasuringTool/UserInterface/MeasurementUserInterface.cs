using Unity.VisualScripting;
using UnityEngine;

/*
 * MeasurementUserInterface
 * This class handles displaying the crosshairs and tool icons to the user.
 */
public class MeasurementUserInterface : MonoBehaviour
{
    [SerializeField] MeasurementToolManager _manager;
    [SerializeField] MeasurementToolIcon _toolIcon;
    [SerializeField] MeasurementCrosshairs _crosshairs;

    private GameObject _canvasObject;
    private Canvas _canvas;

    private void Awake()
    {
        CreateUserInterfaceCanvas();

        _manager.OnToolSwitch += UpdateToolIcon;

        _toolIcon.Draw(_canvas);
        _crosshairs.Draw(_canvas);
    }
    private void OnDisable() { _manager.OnToolSwitch -= UpdateToolIcon; }

    private void CreateUserInterfaceCanvas()
    {
        _canvasObject = new GameObject("Measurement User Interface");
        _canvasObject.transform.SetParent(transform, false);
        _canvas = _canvasObject.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

    private void UpdateToolIcon() { _toolIcon.UpdateIcon(_manager.GetActiveToolIcon()); }
}
