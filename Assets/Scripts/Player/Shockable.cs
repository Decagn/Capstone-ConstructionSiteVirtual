using UnityEngine;

public class Shockable : MonoBehaviour
{
    private Renderer _renderer;
    private Color originalColor;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        originalColor = _renderer.material.color;
    }

    public void OnShock()
    {
        Debug.Log("Shocked!");

        // 变成亮红色（非常明显）
        _renderer.material.color = Color.red;

        // 2秒后恢复（可选，加分）
        Invoke(nameof(ResetColor), 2f);
    }

    void ResetColor()
    {
        _renderer.material.color = originalColor;
    }
}