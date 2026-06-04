using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Lesson Measurements Manager
 * This class uses a file parser to find the pre-defined/lesson measurement points within a file in /Resources
 * then uses a visualiser to display those points in the world.
 */
public class LessonMeasurementsManager : MonoBehaviour
{
    [SerializeField] private LessonMeasurementsPointsVisualiser _visualiser;

    // Mandatory prefix for lesson measurement files.
    [SerializeField] private string _filePrefix = "LessonMeasurements";

    private List<List<Vector3>> _lessonMeasurements = new List<List<Vector3>>();

    private void Awake()
    {
        string _currentScene = SceneManager.GetActiveScene().name;
        // The full file name will look like LessonMeasurements-*insert scene name*
        string _fullFileName = _filePrefix + "-" + _currentScene;

        _lessonMeasurements = LessonMeasurementsFileParser.GetMeasurements(_fullFileName);

        _visualiser.DisplayLessonMeasurements(_lessonMeasurements);
    }

    // Returns a list of lesson measurement points to the PointSelector class
    // to allow snapping to these points.
    public List<Vector3> GetLessonMeasurementPoints()
    {
        List<Vector3> points = new List<Vector3>();

        foreach (List<Vector3> measurement in  _lessonMeasurements)
        {
            foreach (Vector3 point in measurement) 
                points.Add(point);
        }

        return points;
    }

}