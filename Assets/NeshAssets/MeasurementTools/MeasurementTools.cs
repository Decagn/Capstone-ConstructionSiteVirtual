using UnityEngine;
using UnityEngine.InputSystem;

public class MeasurementTools : MonoBehaviour
{
    private InputListener _inputListener;
    private PointSelector _pointSelector;
    private MeasurementVisualiser _measurementVisualiser;
    private ToolCrosshairsVisualiser _crosshairsVisualiser;
    private ToolManager _toolManager;

    [SerializeField] private InputAction _clickButton;
    [SerializeField] private GameObject _pointMarkerPrefab;

    private void Awake()
    {
        _inputListener = gameObject.AddComponent<InputListener>();
        _inputListener.Initialise(_clickButton);

        _pointSelector = gameObject.AddComponent<PointSelector>();
        _pointSelector.Initialise(_inputListener);

        _measurementVisualiser = gameObject.AddComponent<MeasurementVisualiser>();
        _measurementVisualiser.Initialise(_pointMarkerPrefab);

        _crosshairsVisualiser = gameObject.AddComponent<ToolCrosshairsVisualiser>();

        _toolManager = gameObject.AddComponent<ToolManager>();
        _toolManager.Initialise(_pointSelector, _measurementVisualiser);
    }
}
