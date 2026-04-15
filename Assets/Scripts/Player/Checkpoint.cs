using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    /// <summary>
    /// Checks for collision with player and sets current spawnpoint, if collision happened.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Trigger betreten von: " + collision.gameObject.name + " Tag: " + collision.tag);

        if (collision.CompareTag("Player"))
        {
            PlayerRespawn playerRespawn = collision.GetComponentInParent<PlayerRespawn>();
            if (playerRespawn != null)
            {
                // sets checkpoint fur current, running session
                playerRespawn.SetCheckpoint(transform.position);

                // saves coordinates of position permanently for loading of level
                PlayerPrefs.SetFloat("CheckpointX", transform.position.x);
                PlayerPrefs.SetFloat("CheckpointY", transform.position.y);
                PlayerPrefs.Save();

                Debug.Log("Last Checkpoint permanently saved at: " + transform.position);
            }
        }
    }
}
