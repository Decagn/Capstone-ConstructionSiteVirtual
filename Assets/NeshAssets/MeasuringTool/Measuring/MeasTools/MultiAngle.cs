using System.Collections.Generic;
using UnityEngine;

public class MultiAngle : MonoBehaviour, IMeasTool
{
    [SerializeField] Sprite icon;
    public string Name
    {
        get => "MultiAngle";
    }
    public Sprite ToolIcon { get; set; }
    public List<Vector3> SelecPoints { get; private set; }

    private Meas _multiMeas;
    private bool _multiMeasInit = false;

    private void Awake()
    {
        SetIcon();
    }

    public Meas TakeMeas()
    {
        if (!_multiMeasInit)
        {
            Length length = MeasLength();
            _multiMeas = new Meas(this, SelecPoints, length);
            _multiMeasInit = true;
            return _multiMeas;
        }

        _multiMeas.AddPoint(SelecPoints[SelecPoints.Count - 1]);
        _multiMeas.AddQuantity(MeasLength());
        _multiMeas.AddQuantity(MeasAngle());
        return _multiMeas;
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

    public bool ResetAllPoints()
    {
        if (SelecPoints == null || SelecPoints.Count == 0) return false;

        SelecPoints.Clear();

        _multiMeasInit = false;

        return true;
    }

    public void SetIcon()
    {
        ToolIcon = icon;
    }
}
