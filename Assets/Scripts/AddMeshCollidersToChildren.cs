using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AutoAddMeshColliders : MonoBehaviour
{
    public void AddColliders()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter filter in meshFilters)
        {
            GameObject child = filter.gameObject;

            if (child.name.ToLower().Contains("door"))
            {
                continue; 
            }

            MeshCollider collider = child.GetComponent<MeshCollider>();

            if (collider == null)
            {
#if UNITY_EDITOR
                collider = Undo.AddComponent<MeshCollider>(child);
#else
                collider = child.AddComponent<MeshCollider>();
#endif

            }

            if (filter.sharedMesh != null)
            {
                collider.sharedMesh = filter.sharedMesh;
            }
        }
    }
}
