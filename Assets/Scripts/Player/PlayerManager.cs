using UnityEngine;
using UnityEngine.InputSystem;

//This class is responsible for manager what 
public class PlayerManager : MonoBehaviour
{
    //Array of player prefabs that can be spawned in the scene. This allows for easy switching between different player controllers (e.g. first person, third person, etc.) without changing code.
    [Header("Player Prefabs")]
    [SerializeField] private GameObject[] playerPrefabs;

    private GameObject currentPlayer;
    private void Awake()
    {
        currentPlayer = Instantiate(playerPrefabs[0], transform.position, Quaternion.identity);
    }
}
