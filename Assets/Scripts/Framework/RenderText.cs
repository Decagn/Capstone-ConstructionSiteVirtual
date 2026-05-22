using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RenderText : MonoBehaviour
{
    public string modelName;
    public Vector3 playerPos;
    public List<GameObject> modelLabels;

    public void renderText(string lessonFile)
    {
        TextAsset txtData = (TextAsset)Resources.Load($"Lesson Plans/{lessonFile}");

        if (txtData == null)
        {
            Debug.Log("Could not locate lesson plan");
            return;
        }

        LessonLabels labels = LessonLabels.CreateFromJSON(txtData.text);

        modelName = labels.model;
        playerPos = labels.player_pos;

        int i = 1;
        foreach (TextLabel label in labels.labels)
        {
            Debug.Log($"label pos: {label.xyz_pos}, label text: {label.text}");

            GameObject newLabel = new GameObject($"Label{i}");
            newLabel.transform.position = label.xyz_pos;
            Canvas labelCanvas = newLabel.AddComponent<Canvas>();
            Image labelBackdrop = labelCanvas.AddComponent<Image>();

            TextMesh labelText = labelCanvas.AddComponent<TextMesh>();
            labelText.gameObject.layer = 0;
            labelText.text = label.text;
            labelText.fontSize = 8;
            labelText.color = Color.black;

            Bounds textBounds = labelText.GetComponent<Renderer>().bounds;
            labelBackdrop.gameObject.layer = 1;
            Color labelColour = Color.white;
            labelColour.a = 0.1f;
            labelBackdrop.color = labelColour;
            labelBackdrop.rectTransform.pivot = new Vector3(0, 1, 1);
            labelBackdrop.rectTransform.anchorMin = new Vector3(0, 0, 0);
            labelBackdrop.rectTransform.anchoredPosition = new Vector3(label.xyz_pos.x, label.xyz_pos.y, label.xyz_pos.z);
            labelBackdrop.rectTransform.sizeDelta = new Vector3(textBounds.size.x, textBounds.size.y, textBounds.size.z);

            modelLabels.Add(newLabel);
            i++;
        }
    }
}

[System.Serializable]
public class LessonLabels
{
    public string model;
    public Vector3 player_pos;
    public List<TextLabel> labels;

    public static LessonLabels CreateFromJSON(string jsonStr)
    {
        return JsonUtility.FromJson<LessonLabels>(jsonStr);
    }
}

[System.Serializable]
public class TextLabel
{
    public Vector3 xyz_pos;
    public string text;

    public static TextLabel CreateFromJSON(string jsonStr)
    {
        return JsonUtility.FromJson<TextLabel>(jsonStr);
    }
}