using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class RenderAll : MonoBehaviour
{
    public List<GameObject> houseModels;
    int currentLevel = 0;
    GameObject currentModel;
    GameObject player;

    LoadModel loadModel;
    RenderText renderText;
    void Start()
    {
        loadModel = this.GetComponent<LoadModel>();
        renderText = this.GetComponent<RenderText>();

        player = GameObject.Find("Player");

        houseModels = loadModel.loadModel();

        NextLevel();
    }

    void Update()
    {
        // Check if next level requested
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            NextLevel();
        }
    }

    public void NextLevel()
    {
        currentLevel++;

        // Disable any active models
        if (currentModel != null)
        {
            currentModel.SetActive(false);
        }

        if (!File.Exists($"Assets/Resources/Lesson Plans/Lesson{currentLevel}.txt"))
        {
            currentLevel = 1;
        }

        Debug.Log($"Level change requested. New level: {currentLevel}");
        
        // Load correct model for level
        renderText.renderText($"Lesson{currentLevel}");
        currentModel = loadModel.DisplayModel(renderText.modelName);

        // Move player to expected start position
        Debug.Log($"Moving player to position {renderText.playerPos}");
        player.SetActive(false);
        player.transform.position = renderText.playerPos;
        player.SetActive(true);
    }
}
