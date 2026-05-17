using UnityEngine;

public class DisableSelfIfNotMobile : MonoBehaviour
{
    void Start()
    {
        if (Application.isMobilePlatform || UnityEngine.Device.Application.isMobilePlatform) return;
        gameObject.SetActive(false);
    }
}