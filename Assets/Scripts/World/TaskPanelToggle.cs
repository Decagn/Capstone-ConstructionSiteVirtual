using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// TaskPanelToggle - Expands or collapses the entire TaskPanel.
///
/// When collapsed, the panel shrinks to only show the toggle button,
/// and the panel background image is hidden.
/// When expanded, the panel restores its full height and background.
/// Attach to the TaskPanel GameObject.
/// Press T key on desktop to toggle the panel.
/// </summary>
public class TaskPanelToggle : MonoBehaviour
{
    [Tooltip("The ScrollView GameObject inside TaskPanel — hidden when collapsed.")]
    public GameObject scrollView;

    [Tooltip("The button that toggles the panel open and closed.")]
    public Button toggleButton;

    [Tooltip("The label on the toggle button.")]
    public TMP_Text buttonLabel;

    [Tooltip("Height of the panel when expanded.")]
    public float expandedHeight = 400f;

    [Tooltip("Height of the panel when collapsed — should match the button height.")]
    public float collapsedHeight = 40f;

    [Tooltip("Label shown when the panel is expanded.")]
    public string expandedLabel = "Tasks ▲";

    [Tooltip("Label shown when the panel is collapsed.")]
    public string collapsedLabel = "Tasks ▼";

    // Tracks whether the panel is currently expanded
    private bool _isExpanded = true;

    // RectTransform of the TaskPanel itself
    private RectTransform _panelRect;

    // Background Image component on the TaskPanel
    private Image _panelImage;

    private void Start()
    {
        // Cache the RectTransform and Image of this TaskPanel GameObject
        _panelRect = GetComponent<RectTransform>();
        _panelImage = GetComponent<Image>();

        // Register button click listener
        if (toggleButton != null)
            toggleButton.onClick.AddListener(OnToggleClicked);

        // Set initial state to expanded
        UpdatePanel();
    }

    private void Update()
    {
        // Press T key on desktop to toggle the task panel
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
            OnToggleClicked();
    }

    /// <summary>
    /// Called when the toggle button is clicked or T key is pressed.
    /// Flips the expanded state and updates the panel.
    /// </summary>
    private void OnToggleClicked()
    {
        _isExpanded = !_isExpanded;
        UpdatePanel();
    }

    /// <summary>
    /// Updates the panel height, background visibility, scroll view visibility,
    /// and button label to match the current expanded/collapsed state.
    /// </summary>
    private void UpdatePanel()
    {
        // Resize the TaskPanel by changing its sizeDelta height
        if (_panelRect != null)
        {
            Vector2 size = _panelRect.sizeDelta;
            size.y = _isExpanded ? expandedHeight : collapsedHeight;
            _panelRect.sizeDelta = size;
        }

        // Show or hide the panel background image
        if (_panelImage != null)
            _panelImage.enabled = _isExpanded;

        // Show or hide the scroll view
        if (scrollView != null)
            scrollView.SetActive(_isExpanded);

        // Update button label to reflect current state
        if (buttonLabel != null)
            buttonLabel.text = _isExpanded ? expandedLabel : collapsedLabel;
    }
}