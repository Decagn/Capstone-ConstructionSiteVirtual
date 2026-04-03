using UnityEngine;

public interface IMeasurementTool
{
    string ToolName {  get; }
    void OnPointSelected(Vector3 point);
    void ResetMeasurement();
}
