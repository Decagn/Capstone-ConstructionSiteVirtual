using UnityEngine;
using UnityEngine.UI;

public class ToolCrosshairsVisualiser : MonoBehaviour
{
    private GameObject _canvasGameObject;
    private Canvas _canvas;

    [SerializeField] private Sprite _crosshairSprite;

    private void Awake()
    {
        _canvasGameObject = new GameObject("MeasurementTools:CrosshairCanvas");
        _canvas = _canvasGameObject.AddComponent<Canvas>();
    }

}
