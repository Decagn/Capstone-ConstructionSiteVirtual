using System.Collections.Generic;
using UnityEngine;

public class Protractor : MonoBehaviour, IMeasTool
{
    [SerializeField] Sprite icon;
    public string Name
    {
        get => "Protractor";
    }
    public Sprite ToolIcon { get; set; }
    public List<Vector3> SelecPoints { get; private set; }

    private void Awake()
    {
        SetIcon();
    }

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

    public void SetIcon()
    {
        ToolIcon = icon;
    }
}
