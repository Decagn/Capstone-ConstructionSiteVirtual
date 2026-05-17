using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LessonSnapManager : MonoBehaviour
{
    [SerializeField] private LessonSnapVisualiser _visualiser;
    [SerializeField] private string _fileName = "LessonSnapObjects";
    private List<List<Vector3>> _objectVertices = new List<List<Vector3>>();

    private void Awake()
    {
        string _currentScene = SceneManager.GetActiveScene().name;
        string _fullFileName = _currentScene + "-" + _fileName;
        _objectVertices = LessonSnapFileParser.GetVertices(_fullFileName);
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
