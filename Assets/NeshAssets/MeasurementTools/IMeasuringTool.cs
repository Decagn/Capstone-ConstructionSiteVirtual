using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IMeasuringTool
{
    string ToolName { get; }
    List<Vector3> SelectedPoints { get; set; }
    void HandleSelectedPoint(Vector3 selectedPoint);
    void ResetSelectedPoints();
    void AddSelectedPoint(Vector3 selectedPoint);
}
