using UnityEngine;

public interface IMeasurementTool
{
    string Tool {  get; }
    void OnPointSelected(Vector3 point);
    void ResetMeasurement();
}
