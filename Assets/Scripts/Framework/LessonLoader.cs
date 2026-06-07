/// <summary>
/// Master class overseeing resource loading from the lesson framework and runtime lesson generation.
/// </summary>

using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class LessonLoader : MonoBehaviour
{
    int currentLesson = 0;
    DynamicTextures dynamicTextures;
    JsonParser jsonParser;
    GameObject player;
    ModelLoader modelLoader;
    TaskManager taskManager;
    TextRenderer textRenderer;

    void Start()
    {
        // Attaching key components to variables at runtime
        dynamicTextures = this.GetComponent<DynamicTextures>();
        jsonParser = this.GetComponent<JsonParser>();
        modelLoader = this.GetComponent<ModelLoader>();
        textRenderer = this.GetComponent<TextRenderer>();
        taskManager = GameObject.Find("TaskManager").GetComponent<TaskManager>();
        player = GameObject.Find("PlayerController");

        // Load first level
        NextLevel();
    }

    void Update()
    {
        // Check if next level requested using the `m` key
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            NextLevel();
        }
    }

    public void NextLevel()
    {
        currentLesson++;

        // If number of available lesson plans exceded, wrap around to 1st lesson
        if (!File.Exists($"Assets/Resources/Lesson Plans/Lesson{currentLesson}.json"))
        {
            currentLesson = 1;
        }

        // Parse JSON lesson plan for current lesson
        Debug.Log($"Level change requested. Parsing lesson plan for Lesson {currentLesson}");
        TextAsset lessonFile = (TextAsset)Resources.Load($"Lesson Plans/Lesson{currentLesson}");
        LessonPlan currentPlan = jsonParser.ParseLesson(lessonFile.text);

        // Temporary checking of interactive objects loaded
        Debug.Log($"Loading {currentPlan.interactiveObjects.Count} interactive objects...");
        foreach(var interactiveObject in currentPlan.interactiveObjects)
        {
            Debug.Log($"Interactive object loaded for {interactiveObject.gameObject}, with: " +
                        $"elementName={interactiveObject.elementName}, " +
                        $"material={interactiveObject.material}, " +
                        $"description={interactiveObject.description}, " +
                        $"elementId={interactiveObject.elementId}");
        }

        // Display correct house model for current lesson and clear existing model
        Debug.Log($"Loading house model: {currentPlan.modelName}");
        modelLoader.DisplayModel(currentPlan.modelName, currentPlan.interactiveObjects);

        // Allocate any dynamically-loaded textures within the model
        Debug.Log("Allocating dynamic textures...");
        dynamicTextures.TextureAdder(currentPlan.modelName);

        // Set up tasks for the current lesson
        Debug.Log($"Loading tasks with lesson title: {currentPlan.lessonConfig.lessonTitle}");
        taskManager.lessonConfig = currentPlan.lessonConfig;
        
        // Render text labels for current lesson and clear existing text
        Debug.Log($"Loading text labels for Lesson {currentLesson}");
        textRenderer.RenderText(currentPlan.labels);

        // Move player to expected start position
        Debug.Log($"Moving player to position {currentPlan.playerPos}");
        player.SetActive(false);
        player.transform.position = currentPlan.playerPos;
        player.SetActive(true);
    }
};