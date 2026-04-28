using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class POILogBook
{
    private List<Vector3> _POIs = new List<Vector3>();

    public void Add(Vector3 POI)
    {
        _POIs.Add(POI);
    }
}
