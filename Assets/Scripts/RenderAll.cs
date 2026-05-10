using UnityEngine;
using System.Linq;

public class RenderAll : MonoBehaviour
{
    public GameObject[] houseModels;
    int currentLevel = 0;

    LoadModel loadModel;
    RenderText renderText;
    void Start()
    {
        loadModel = this.GetComponent<LoadModel>();
        renderText = this.GetComponent<RenderText>();

        houseModels = loadModel.loadModel();

        NextLevel();
    }

    public void NextLevel()
    {
        currentLevel++;

        // Load correct model for level
        renderText.renderText($"Lesson{currentLevel}");
        loadModel.DisplayModel(renderText.modelName);
    }
}
