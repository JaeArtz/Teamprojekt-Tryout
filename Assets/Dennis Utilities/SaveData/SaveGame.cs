using UnityEngine;

public class SaveGame : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private SoulManager soulManager;
    private CollectableManager collectableManager;
    private PlayerRespawn playerRespawn;
    private void Awake()
    {
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();

        soulManager = GameObject.Find("GameManager").GetComponent<SoulManager>();
        collectableManager = GameObject.Find("GameManager").GetComponent<CollectableManager>();

        playerRespawn = GameObject.Find("Player").GetComponent<PlayerRespawn>();
    }

    public void SaveCurrentGame()
    {
        SaveSystem.SaveData(playerHealth, playerRespawn);
    }
}
