using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/*
 * Visualised Object Struct
 * All visualised objects (point markers, lines, arcs, pop-up texts, etc) must inherit this.
 */
public struct IVisualisedObject
{ 
    public VisualType visualType;
    public GameObject obj;
    public List<Vector3> points;

    public IVisualisedObject(VisualType visualType, GameObject obj, List<Vector3> points)
    {
        this.visualType = visualType;
        this.obj = obj;
        this.points = points;
    }
}

public enum VisualType
{ 
    NewPointMarker,
    PointMarker,
    Line,
    Arc,
    PopupText
}