/// <summary>
/// Renders the text labels specified in the lesson plan for the current lesson.
/// </summary>

using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TextRenderer : MonoBehaviour
{
    // Get Camera for text alignment
    [SerializeField] Camera _playerCamera;
    public List<GameObject> modelLabels;

    void Start()
    {
        modelLabels = new List<GameObject>();
    }

    private void Update()
    {
        // Make text face player
        foreach(GameObject model in modelLabels)
            model.transform.forward = _playerCamera.transform.forward;
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

            RectTransform rect = newLabel.AddComponent<RectTransform>();
            rect.transform.position = label.labelPos;
            rect.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            Canvas labelCanvas = newLabel.AddComponent<Canvas>();
            labelCanvas.renderMode = RenderMode.WorldSpace;
            labelCanvas.worldCamera = UnityEngine.Camera.main;

            TextMeshProUGUI newText = labelCanvas.AddComponent<TextMeshProUGUI>();
            newText.text = label.text;
            newText.fontSize = label.fontSize;
            newText.alignment = TextAlignmentOptions.Center;
            modelLabels.Add(newLabel);
        }    
    }
}
