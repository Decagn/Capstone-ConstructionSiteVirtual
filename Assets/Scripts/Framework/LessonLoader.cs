/// <summary>
/// Master class overseeing resource loading from the lesson framework and runtime lesson generation.
/// </summary>

using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class LessonLoader : MonoBehaviour
{
    int currentLesson = 0;
    JsonParser jsonParser;
    GameObject player;
    ModelLoader modelLoader;
    TaskManager taskManager;
    TextRenderer textRenderer;

    void Start()
    {
        // Attaching key components to variables at runtime
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
        if (!File.Exists($"Assets/Resources/Lesson Plans/Lesson{currentLesson}.txt"))
        {
            currentLesson = 1;
        }

        // Parse JSON lesson plan for current lesson
        Debug.Log($"Level change requested. Parsing lesson plan for Lesson {currentLesson}");
        TextAsset lessonFile = (TextAsset)Resources.Load($"Lesson Plans/Lesson{currentLesson}");
        LessonPlan currentPlan = jsonParser.ParseLesson(lessonFile.text);

        // Display correct house model for current lesson and clear existing model
        Debug.Log($"Loading house model: {currentPlan.modelName}");
        modelLoader.DisplayModel(currentPlan.modelName);

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