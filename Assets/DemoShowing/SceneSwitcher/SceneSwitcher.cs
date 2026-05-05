using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] private string[] _sceneNames;
    [SerializeField] private InputAction _switchScene;
    private int _currentSceneIndex = 0;

    public void Awake()
    {
        if (_switchScene.bindings.Count == 0) _switchScene.AddBinding("<Keyboard>/t");

        string currentScene = SceneManager.GetActiveScene().name;
        for (int i = 0; i < _sceneNames.Length; i++)
        {
            if (_sceneNames[i] == currentScene)
            {
                _currentSceneIndex = i;
                break;
            }
        }
    }

    private void OnEnable()
    {
        _switchScene.Enable();
    }
    private void OnDisable()
    {
        _switchScene.Disable();
    }

    private void Update()
    {
        if (_switchScene.WasPressedThisFrame())
        {
            SwitchToNextScene();
        }
    }

    private void SwitchToNextScene()
    {
        _currentSceneIndex = (_currentSceneIndex + 1) % _sceneNames.Length;
        string nextScene = _sceneNames[_currentSceneIndex];
        Debug.Log($"SceneSwitcher: Switching to scene --> {nextScene}");
        SceneManager.LoadScene(nextScene);
    }
}
