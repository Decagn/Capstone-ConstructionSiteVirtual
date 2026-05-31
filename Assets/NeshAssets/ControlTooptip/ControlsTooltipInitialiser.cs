using UnityEngine;

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
        "Toggle Snapping";

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
        "[ Z ] - ";

    private void Awake()
    {
        ControlsTooltip tooltip = gameObject.AddComponent<ControlsTooltip>();
        if (!(Application.isMobilePlatform || UnityEngine.Device.Application.isMobilePlatform))
        {
            tooltip.Initialise(_controls, _keys);
        }
    }
}