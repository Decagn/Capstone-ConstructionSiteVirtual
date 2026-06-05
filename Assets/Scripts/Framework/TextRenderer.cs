/// <summary>
/// Renders the text labels specified in the lesson plan for the current lesson.
/// </summary>

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextRenderer : MonoBehaviour
{
    public List<GameObject> modelLabels;

    void Start()
    {
        modelLabels = new List<GameObject>();
    }

    public void RenderText(List<TextLabel> labelData)
    {
        // Clear existing text labels
        modelLabels.Clear();        

        // Generate labels from JSON data
        foreach(var label in labelData)
        {
            Debug.Log($"Adding label '{label.text}' at position {label.labelPos}");
            GameObject newLabel = new GameObject($"Label: {label.text}");
            newLabel.transform.position = label.labelPos;

            TextMeshPro newText = newLabel.AddComponent<TextMeshPro>();
            newText.transform.SetParent(newLabel.transform);
            newText.text = label.text;
            newText.fontSize = label.fontSize;
            modelLabels.Add(newLabel);
        }    
    }
}
