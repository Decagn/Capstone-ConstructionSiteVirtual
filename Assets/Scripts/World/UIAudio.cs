using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIAudio - Automatically adds click sound to all Button components
/// that are children of this GameObject.
/// Attach to the Canvas or Panel you want to apply sounds to.
/// </summary>
public class UIAudio : MonoBehaviour
{
    private void Start()
    {
        // Only find buttons that are children of this GameObject
        Button[] buttons = GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() =>
            {
                AudioManager.Instance?.PlayUIClick();
            });
        }

        Debug.Log($"[UIAudio] Added click sound to {buttons.Length} buttons under {gameObject.name}.");
    }
}