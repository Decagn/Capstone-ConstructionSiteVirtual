using GLTFast.Schema;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;

/*
 * Pop-up Texts
 * These differ from the other visualised objects
 * because they must be instantiated based on other visualised objects (lines, angles, etc.)
 */
public class PopupTextVisualiser : MonoBehaviour, IVisualiser
{
    [SerializeField] UnityEngine.Material _popupMaterial;
    [SerializeField] float _popupSize = 0.005f;
    [SerializeField] float _fontSize = 20f;

    public List<IVisualisedObject> Create(List<Vector3> points) => new List<IVisualisedObject>();

    private GameObject CreateTextPopup(GameObject obj, Vector3 position, string measurementText)
    {
        GameObject popup = new GameObject("Measurement Text Pop-up");
        popup.transform.SetParent(obj.transform);

        RectTransform rect = popup.AddComponent<RectTransform>();
        rect.transform.localPosition = position;
        rect.localScale = new Vector3(_popupSize, _popupSize, _popupSize);

        Canvas popupCanvas = popup.AddComponent<Canvas>();
        popupCanvas.renderMode = RenderMode.WorldSpace;
        popupCanvas.worldCamera = UnityEngine.Camera.main;

        TextMeshProUGUI text = popupCanvas.AddComponent<TextMeshProUGUI>();
        text.fontSize = _fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = UnityEngine.Color.white;
        text.fontSharedMaterial = _popupMaterial;
        text.text = measurementText;

        return popup;
    }

    private IVisualisedObject CreateLineTextPopup(IVisualisedObject line)
    {
        Vector3 pointA = line.points[0];
        Vector3 pointB = line.points[1];

        Vector3 popupPosition = (pointA + pointB) / 2;
        float measurementValue = Calculator.GetLength(pointA, pointB);
        string measurementText = $"{measurementValue:F2}m";
        GameObject popupObj = CreateTextPopup(line.obj, popupPosition, measurementText);

        return new IVisualisedObject(VisualType.PopupText, popupObj, line.points);
    }

    private IVisualisedObject CreateArcTextPopup(IVisualisedObject arc)
    {
        Vector3 pointA = arc.points[0];
        Vector3 vertex = arc.points[1];
        Vector3 pointB = arc.points[2];

        Vector3 popupPosition = vertex;
        float val = Calculator.GetAngle(pointA, vertex, pointB);
        string measText = $"{val:F1}°";
        GameObject popupObj = CreateTextPopup(arc.obj, popupPosition, measText);
        
        return new IVisualisedObject(VisualType.PopupText, popupObj, arc.points);
    }

    // Finds the center of a polygon with its vertices defined by a list of Vector3s.
    public Vector3 GetCentroid(List<Vector3> points)
    {
        if (points == null || points.Count == 0)
            return Vector3.zero;

        Vector3 sum = Vector3.zero;
        for (int i = 0; i < points.Count; i++)
            sum += points[i];

        return sum / points.Count;
    }

    public List<IVisualisedObject> CreateAreaTextPopups(List<Vector3> points, GameObject parent)
    {
        List<IVisualisedObject> areaPopups = new List<IVisualisedObject>();
        List<Area> areas = TakeAreas(points);

        foreach (Area area in areas)
        {
            Vector3 centroid = GetCentroid(area.GetPoints);
            string measText = $"{area.Value:F2}m²";
            GameObject popupObj = CreateTextPopup(parent, centroid, measText);
            areaPopups.Add(new IVisualisedObject(VisualType.PopupText, popupObj, area.GetPoints));
        }

        return areaPopups;
    }

    private List<Area> TakeAreas(List<Vector3> points)
    {
        List<List<Vector3>> polygons = new List<List<Vector3>>();
        for (int p = 0; p < points.Count - 3; p++)
        {
            List<Vector3> polygon = new List<Vector3>();
            // finds the next index where the same point was selected.
            // the next index has to be at minimum 3 away so it can at least form a triangle.
            for (int p2 = points.Count - 1; p2 > p + 2; p2--)
            {
                if (points[p2] == points[p])
                {
                    polygon.AddRange(points.GetRange(p, p2 - p));
                    polygons.Add(polygon);
                }
            }
        }

        List<Area> areas = new List<Area>();
        foreach (List<Vector3> polygon in polygons)
            areas.Add(new Area(polygon));
        return areas;
    }

    public List<IVisualisedObject> CreatePopupTexts(List<IVisualisedObject> objects)
    {
        List<IVisualisedObject> popupsTexts = new List<IVisualisedObject>();

        foreach (IVisualisedObject obj in objects)
        {
            if (obj.visualType == VisualType.Line)
                popupsTexts.Add(CreateLineTextPopup(obj));

            else if (obj.visualType == VisualType.Arc)
                popupsTexts.Add(CreateArcTextPopup(obj));

            else
                continue;
        }

        return popupsTexts;
    }

    public void Scale(IVisualisedObject obj, UnityEngine.Camera camera)
    { Scaler.ScaleText(camera, obj.obj, _fontSize); }
}
