using UnityEngine;
using UnityEngine.InputSystem;

//This class is responsible for manager what the current player controller is being used, and enables the ability to switch between them.
public class PlayerManager : MonoBehaviour
{
    //Array of player prefabs that can be spawned in the scene. This allows for easy switching between different player controllers (e.g. first person, third person, etc.) without changing code.
    [Header("Player Prefabs")]
    [SerializeField] private GameObject[] playerPrefabs;

    private GameObject currentPlayer;
    private int currentPlayerIndex = 0;
    private void Awake()
    {
        currentPlayer = Instantiate(playerPrefabs[0], transform.position, transform.rotation);
    }

    public void Update()
    {
        if(Keyboard.current.qKey.wasPressedThisFrame) SwitchPlayer();
    }

    // Switches to the next player prefab in array after destroying current player prefab.
    public void SwitchPlayer()
    {
        GameObject previousPlayer = currentPlayer;
        currentPlayerIndex = (currentPlayerIndex + 1) % playerPrefabs.Length;
        Destroy(currentPlayer);
        currentPlayer = Instantiate(playerPrefabs[currentPlayerIndex], previousPlayer.transform.position, previousPlayer.transform.rotation);
    }
}
