using UnityEngine;

public class PointSelector : MonoBehaviour
{
    [SerializeField] float _furthestPoint = Mathf.Infinity;

    public Vector3 GetPoint(Vector2 screenPoint)
    {
        (Vector3 point, RaycastHit hit) = TryGetPoint(screenPoint);

        return point;
    }

    private (Vector3 point, RaycastHit ray) TryGetPoint(Vector2 screenPoint)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPoint);
        Physics.Raycast(ray, out RaycastHit hit, _furthestPoint);
        return (hit.point, hit);
    }
}
