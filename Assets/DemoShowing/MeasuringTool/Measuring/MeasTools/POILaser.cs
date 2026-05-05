using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * Point-of-interest (POI) Laser Tool
 * Used in lesson creation to pick out points in the scene.
 */
public class POILaser : MonoBehaviour, IMeasTool
{
    [SerializeField] Sprite icon;
    public event Action<Vector3> OnPointSelected;
    public event Action OnLastPointDeselected;
    public event Action OnAllPointsResetted;

    public string Name
    {
        get => "Point-Of-Interest Laser";
    }
    public Sprite ToolIcon { get; set; }
    public List<Vector3> SelecPoints { get; private set; }

    private void Awake()
    {
        SetIcon();
        SelecPoints = new List<Vector3>();
    }

    /*
     * TakeMeas is never called for POILaser
     * So the contents for this method are just placeholders
     * to fulfill the IMeasTool interface requirements.
     */
    public Meas TakeMeas()
    {
        Length length = new Length(Vector3.zero, Vector3.zero);
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
        MeasDebug.Log($"Point: {point} selected", "POILaser");
        OnPointSelected?.Invoke(point);

        return new InvalidMeas();
    }

    public bool RemoveLastPoint()
    {
        if (SelecPoints == null || SelecPoints.Count == 0) return false;

        MeasDebug.Log($"Last point: {SelecPoints[SelecPoints.Count - 1]} deselected", "POILaser");
        OnLastPointDeselected?.Invoke();

        SelecPoints.RemoveAt(SelecPoints.Count - 1);
        return true;
    }
    public bool ResetAllPoints()
    {
        if (SelecPoints == null || SelecPoints.Count == 0) return false;

        MeasDebug.Log($"All points removed", "POILaser");
        OnAllPointsResetted?.Invoke();

        SelecPoints.Clear();
        return true;
    }

    public void SetIcon()
    {
        ToolIcon = icon;
    }
}