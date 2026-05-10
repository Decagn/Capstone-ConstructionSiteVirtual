using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LessonSnapManager : MonoBehaviour
{
    [SerializeField] private LessonSnapVisualiser _visualiser;
    [SerializeField] private string _fileName = "LessonSnapObjects";

    private List<List<Vector3>> _objectVertices = new List<List<Vector3>>();

    private void Awake()
    {
        _objectVertices = LessonSnapFileParser.GetVertices(_fileName);
        _visualiser.DrawLessonSnapObjects(_objectVertices);
    }

    public List<Vector3> GetPoints()
    {
        List<Vector3> points = new List<Vector3>();
        foreach (List<Vector3> objectVertices in  _objectVertices)
        {
            foreach (Vector3 vertex in objectVertices) points.Add(vertex);
        }
        return points;
    }
}
