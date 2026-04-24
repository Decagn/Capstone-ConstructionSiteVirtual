using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public interface IMeasTool
{
    string Name { get; }
    Sprite ToolIcon { get; set; }
    List<Vector3> SelecPoints { get; }
    Meas TakeMeas();
    IMeas TakePoint(Vector3 point);
    bool RemoveLastPoint()
    {
        if (SelecPoints == null || SelecPoints.Count == 0) return false;

        SelecPoints.RemoveAt(SelecPoints.Count - 1);
        return true;
    }
    bool ResetAllPoints()
    {
        if (SelecPoints == null || SelecPoints.Count == 0) return false;

        SelecPoints.Clear();
        return true;
    }
    void SetIcon();
}
