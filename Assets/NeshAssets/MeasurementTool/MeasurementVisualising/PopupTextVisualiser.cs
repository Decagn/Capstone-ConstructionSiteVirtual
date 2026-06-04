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

    public List<IVisualisedObject> CreateAreaTextPopups(Measurement measurement, GameObject parent)
    {
        List<IQuantity> quantities = measurement.GetQuantities();
        List <IVisualisedObject> areaPopups = new List<IVisualisedObject>();

        foreach (IQuantity quantity in quantities)
        {
            if (quantity.Type != QuantityType.Area)
                continue;

            List<Vector3> polygonVertices = ((Area)quantity).GetPoints;
            Vector3 centroid = GetCentroid(polygonVertices);
            string measText = $"{quantity.Value:F2}m²";
            GameObject popupObj = CreateTextPopup(parent, centroid, measText);
            areaPopups.Add(new IVisualisedObject(VisualType.PopupText, popupObj, polygonVertices));
        }

        return areaPopups;
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
