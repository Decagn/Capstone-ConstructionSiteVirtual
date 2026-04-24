using System.Collections.Generic;
using UnityEngine;

public class MultiAngle : IMeasTool
{
    public string Name
    {
        get => "MultiAngle";
    }
    public Sprite ToolIcon { get; }
    public List<Vector3> SelecPoints { get; private set; }

    private Meas multiMeas;
    private bool multiMeasInit = false;

    public Meas TakeMeas()
    {
        if (!multiMeasInit)
        {
            Length length = MeasLength();
            multiMeas = new Meas(this, SelecPoints, length);
            return multiMeas;
        }

        multiMeas.AddPoint(SelecPoints[SelecPoints.Count - 1]);
        multiMeas.AddQuantity(MeasLength());
        multiMeas.AddQuantity(MeasAngle());
        return multiMeas;
    }

    private Length MeasLength()
    {
        return new Length(SelecPoints[SelecPoints.Count - 1], 
            SelecPoints[SelecPoints.Count - 2]);
    }

    private Angle MeasAngle()
    {
        return new Angle(SelecPoints[SelecPoints.Count - 1], 
            SelecPoints[SelecPoints.Count - 2], 
            SelecPoints[SelecPoints.Count - 3]);
    }

    public IMeas TakePoint(Vector3 point)
    {
        if (SelecPoints == null)
        {
            SelecPoints = new List<Vector3>();
        }

        SelecPoints.Add(point);

        if (SelecPoints.Count < 2) return new InvalidMeas();

        Meas meas = TakeMeas();

        if (SelecPoints.Count < 3) SelecPoints.RemoveRange(0, SelecPoints.Count - 2);
        else SelecPoints.RemoveRange(0, SelecPoints.Count - 3);

        return meas;
    }
}
