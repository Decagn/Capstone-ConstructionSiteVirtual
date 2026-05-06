using UnityEngine;

/// <summary>
/// CursorManager - Replaces the default cursor with a custom dot cursor.
/// Attach to any persistent GameObject in the scene.
/// </summary>
public class CursorManager : MonoBehaviour
{
    [Tooltip("The custom cursor texture — should be a small dot PNG, 32x32px.")]
    public Texture2D cursorTexture;

    private void Start()
    {
        if (cursorTexture != null)
        {
            // Set hotspot to centre of the texture so the click point is the dot centre
            Vector2 hotspot = new Vector2(cursorTexture.width / 2f, cursorTexture.height / 2f);
            Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
        }
    }
}