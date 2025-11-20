using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    /// <summary>
    /// Checks for collision with player and sets current spawnpoint, if collision happened.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        { 
            PlayerRespawn playerRespawn = collision.GetComponent<PlayerRespawn>();
            if (playerRespawn != null)
            {
                playerRespawn.SetCheckpoint(transform.position);
            }
        }
    }
}
