using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IMeasuringTool
{
    string ToolName { get; }
    Sprite ToolIcon { get; set; }
    List<Vector3> SelectedPoints { get; set; }
    List<float> MeasuredLengths { get; set; }
    List<float> MeasuredAngles { get; set; }
    event Action OnSelectedPointsUpdated;
    event Action OnSelectedPointsReseted;
    void Initialise(Sprite toolIcon);
    void HandleSelectedPoint(Vector3 selectedPoint);
    void ResetSelectedPoints();
    void AddSelectedPoint(Vector3 selectedPoint);
}
