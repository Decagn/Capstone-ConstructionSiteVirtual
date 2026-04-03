using UnityEngine;
using System;
using System.Collections.Generic;

public class MeasurementToolManager : MonoBehaviour
{
    /*
     * SYSTEM DESIGN
     * Tool indices are arbritrary
     * However, remember to add them to the list in Awake() in the same order of the indices below
     */
    private const int _protractorRulerIndex = 0;
    
    private IMeasurementTool _activeTool;
    private List<IMeasurementTool> _tools = new List<IMeasurementTool>();

    [SerializeField] private MeasurementPointSelector _pointSelector;

    public event Action<MeasurementCollection> OnMeasurementsUpdated;
    public event Action OnResetMeasurements;

    private void Awake()
    {
        ProtractorRuler protractorRuler = gameObject.AddComponent<ProtractorRuler>();
        _tools.Add(protractorRuler);

        /*
         * SYSTEM DESIGN
         * Protractor Ruler is the default tool, therefore its the first selected
         */
        SelectTool(_protractorRulerIndex);
    }

    private void OnEnable()
    {
        _pointSelector.OnPointSelected += HandlePointSelected;
    }

    private void OnDisable()
    {
        _pointSelector.OnPointSelected -= HandlePointSelected;
    }

    private void SelectTool(int toolIndex)
    {
        if (_activeTool != null)
        {
            _activeTool.OnMeasurementsUpdated -= UpdateMeasurements;

        }

        _activeTool?.ResetMeasurements();
        _activeTool = _tools[toolIndex];
        OnResetMeasurements?.Invoke();
    }

    private void UpdateMeasurements(MeasurementCollection collection)
    {
        OnMeasurementsUpdated?.Invoke(collection);
    }

    private void ResetMeasurement()
    {
        _activeTool?.ResetMeasurements();
        OnResetMeasurements?.Invoke();
    }

    private void HandlePointSelected(Vector3 selectedPoint)
    {
        _activeTool?.OnPointSelected(selectedPoint);
    }
}
