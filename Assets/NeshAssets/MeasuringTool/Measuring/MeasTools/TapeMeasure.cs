using System.Collections.Generic;
using UnityEngine;

public class TapeMeasure : MonoBehaviour, IMeasTool
{
    [SerializeField] Sprite icon;
    public string Name
    {
        get => "Tape Measure";
    }
    public Sprite ToolIcon { get; set; }
    public List<Vector3> SelecPoints { get; private set; }

    private void Awake()
    {
        SetIcon();
        SelecPoints = new List<Vector3>();
    }

    public Meas TakeMeas()
    {
        Length length = new Length(SelecPoints[0], SelecPoints[1]);
        Meas meas = new Meas(this, SelecPoints, length);
        return meas;
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
        SelecPoints.Clear();
        return meas;
    }

    public void SetIcon()
    {
        ToolIcon = icon;
    }
}
