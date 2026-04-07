using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MeasurementTools : MonoBehaviour
{
    private InputListener _inputListener;
    private PointSelector _pointSelector;
    private MeasurementVisualiser _measurementVisualiser;
    private ToolCrosshairsVisualiser _crosshairsVisualiser;
    private ToolManager _toolManager;

    [Header("User Input")]
    [SerializeField] private InputAction _clickButton;
    [Header("Visualing Measurements")]
    [SerializeField] private Material _lineMaterial;
    [Header("Visualing Crosshairs & UX")]
    [SerializeField] private GameObject _pointMarkerPrefab;
    [SerializeField] private Sprite _crosshairsSprite;
    [SerializeField] private Sprite _tapeMeasureIcon;

    private void Awake()
    {
        _inputListener = gameObject.AddComponent<InputListener>();
        _inputListener.Initialise(_clickButton);

        _pointSelector = gameObject.AddComponent<PointSelector>();
        _pointSelector.Initialise(_inputListener);

        _toolManager = gameObject.AddComponent<ToolManager>();
        _toolManager.Initialise(_pointSelector, _tapeMeasureIcon);

        _measurementVisualiser = gameObject.AddComponent<MeasurementVisualiser>();
        _measurementVisualiser.Initialise(_toolManager, _pointMarkerPrefab, _lineMaterial);

        _crosshairsVisualiser = gameObject.AddComponent<ToolCrosshairsVisualiser>();
        _crosshairsVisualiser.Initialise(_toolManager, _crosshairsSprite);
    }
}
