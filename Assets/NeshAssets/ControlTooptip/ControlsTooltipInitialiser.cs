using UnityEngine;

/*
 * Controls Tooltip Initialiser
 * This class initiliases the tool tips
 * that shows the user the controls and the keys the map to.
 */
public class ControlsTooltipInitialiser : MonoBehaviour
{
    private const string _controls =
        "Toggle Fullscreen\n" +
        "Look Around\n" +
        "Move Around\n" +
        "Fly Up/Down\n" +
        "Inspect Object\n" +
        "Toggle Task Menu\n" +
        "Select Point\n" +
        "Deselect Point\n" +
        "Reset All Points\n" +
        "Switch Tool\n" +
        "Switch Models\n" +
        "Toggle Snapping\n" +
        "Show Instructions";

    private const string _keys =
        "[ F ] - \n" +
        "[ Mouse ] - \n" +
        "[ WASD ] - \n" +
        "[ Q/E ] - \n" +
        "[ Right-Click ] - \n" +
        "[ T ] - \n" +
        "[ Left-Click ] - \n" +
        "[ X ] - \n" +
        "[ R ] - \n" +
        "[ Scroll ] - \n" +
        "[ M ] - \n" +
        "[ Z ] - \n" +
        "[ H ] - ";

    private void Awake()
    {
        ControlsTooltip tooltip = gameObject.AddComponent<ControlsTooltip>();

        // Only show the tooltip for desktop users, since mobile controls are intuitive.
        if (!(Application.isMobilePlatform || UnityEngine.Device.Application.isMobilePlatform))
            tooltip.Initialise(_controls, _keys);
    }
}