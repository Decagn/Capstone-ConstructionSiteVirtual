using System.Collections.Generic;
using UnityEngine;

public class Protractor : IMeasTool
{
    public string Name
    {
        get => "Protractor";
    }
    public Sprite ToolIcon { get; }
    public List<Vector3> SelecPoints { get; private set; }

    public Meas TakeMeas()
    {
        Angle angle = new Angle(SelecPoints[0], SelecPoints[1], SelecPoints[2]);
        Meas meas = new Meas(this, SelecPoints, angle);
        return meas;
    }

    public IMeas TakePoint(Vector3 point)
    {
        if (SelecPoints == null)
        {
            SelecPoints = new List<Vector3>();
        }

        SelecPoints.Add(point);

        if (SelecPoints.Count < 3) return new InvalidMeas();

        Meas meas = TakeMeas();
        SelecPoints.Clear();
        return meas;
    }
}
