using UnityEngine;

public class PlayerPersistence : MonoBehaviour
{
    private PlayerRespawn playerRespawn;

    void Awake()
    {
        // gets the already existing PlayerRespawn Script
        playerRespawn = GetComponent<PlayerRespawn>();
    }

    void Start()
    {
        // adds someting to the Start() sequence: checks if an old Checkpoint is saved up
        if (PlayerPrefs.HasKey("CheckpointX") && PlayerPrefs.HasKey("CheckpointY"))
        {
            float x = PlayerPrefs.GetFloat("CheckpointX");
            float y = PlayerPrefs.GetFloat("CheckpointY");
            Vector3 savedPos = new Vector3(x, y, transform.position.z);

            // 1. Teleports Player
            transform.position = savedPos;

            // 2. Informs the PlayerRespawn-Script about coordinates of "Loading Position"
            if (playerRespawn != null)
            {
                playerRespawn.SetCheckpoint(savedPos);
            }

            Debug.Log("Persistence: Old Checkpoint was successfully loaded.");
        }
    }

    // In case of Checkpoint-Trigger
    public void SaveCheckpoint(Vector3 pos)
    {
        PlayerPrefs.SetFloat("CheckpointX", pos.x);
        PlayerPrefs.SetFloat("CheckpointY", pos.y);
        PlayerPrefs.Save();

        // update RespawnPoint in PlayerRespawn Script
        if (playerRespawn != null)
        {
            playerRespawn.SetCheckpoint(pos);
        }
    }
}