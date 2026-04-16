using System.Collections.Generic;
using UnityEngine;

public class PointMarkers : MonoBehaviour
{
    [SerializeField] private GameObject _markerPrefab;
    private List<GameObject> _markers = new List<GameObject>();

    public void CreateMarkers(List<Vector3> points)
    {
        Clear();
        foreach (Vector3 point in points) CreateAt(point);
    }
    private void CreateAt(Vector3 point)
    {
        GameObject pointMarker = Instantiate(_markerPrefab);
        pointMarker.transform.position = point;
        _markers.Add(pointMarker);
    }
    private void Clear()
    {
        foreach (GameObject pointMarker in _markers)
        {
            Destroy(pointMarker);
        }
        _markers.Clear();
    }
}
