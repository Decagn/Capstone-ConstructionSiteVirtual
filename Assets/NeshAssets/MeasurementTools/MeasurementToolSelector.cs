using UnityEngine;
using System;
using System.Collections.Generic;

public class MeasurementToolSelector : MonoBehaviour
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

    public event Action<float> OnMeasurementComplete;
    public event Action OnResetMeasurement;

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
        _activeTool?.ResetMeasurement();
        _activeTool = _tools[toolIndex];
        OnResetMeasurement?.Invoke();
    }

    private void ResetMeasurement()
    {
        _activeTool?.ResetMeasurement();
        OnResetMeasurement?.Invoke();
    }

    private void HandlePointSelected(Vector3 selectedPoint)
    {
        _activeTool?.OnPointSelected(selectedPoint);
    }
}
