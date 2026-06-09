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
    LessonMeasurementsManager lessonMeasurementsManager;

    void Start()
    {
        // Attaching key components to variables at runtime
        dynamicTextures = this.GetComponent<DynamicTextures>();
        jsonParser = this.GetComponent<JsonParser>();
        modelLoader = this.GetComponent<ModelLoader>();
        textRenderer = this.GetComponent<TextRenderer>();
        taskManager = GameObject.Find("TaskManager").GetComponent<TaskManager>();
        player = GameObject.Find("PlayerController");

        lessonMeasurementsManager = FindFirstObjectByType<LessonMeasurementsManager>();
        if (lessonMeasurementsManager == null)
            Debug.LogWarning("[LessonLoader] No LessonMeasurementsManager found in scene.");

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

        // File.Exists is not used here because file system APIs are unavailable in WebGL builds.
        TextAsset lessonFile = (TextAsset)Resources.Load($"LessonPlans/Lesson{currentLesson}");
        if (lessonFile == null)
        {
            currentLesson = 1;
            lessonFile = (TextAsset)Resources.Load($"LessonPlans/Lesson{currentLesson}");
        }

        // Parse JSON lesson plan for current lesson
        Debug.Log($"Level change requested. Parsing lesson plan for Lesson {currentLesson}");
        LessonPlan currentPlan = jsonParser.ParseLesson(lessonFile.text);

        // Pass measurements to lesson measurements manager
        lessonMeasurementsManager?.LoadMeasurements(currentPlan.lessonConfig.rooms);

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