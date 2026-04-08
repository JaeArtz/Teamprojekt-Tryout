using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 respawnPoint;
    public bool useCheckpoints = true;

    
    /// <summary>
    /// Is called once before the first execution of Update after the MonoBehaviour is created.
    /// </summary>
    void Start()
    {
        //respawnPoint = transform.position;
        if (useCheckpoints && PlayerPrefs.HasKey("CheckpointX"))
        {
            float x = PlayerPrefs.GetFloat("CheckpointX");
            float y = PlayerPrefs.GetFloat("CheckpointY");
            respawnPoint = new Vector3(x, y, transform.position.z);
            transform.position = respawnPoint; // Spieler direkt hinsetzen!
        }
        else
        {
            PlayerPrefs.DeleteKey("CheckpointX");
            PlayerPrefs.DeleteKey("CheckpointY");
            respawnPoint = transform.position; // Fallback: Levelstart
        }
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        respawnPoint = newCheckpoint;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("FallDetector"))
        { 
            transform.position = respawnPoint;

            var myRigidBody = GetComponentInParent<Rigidbody2D>();
            if (myRigidBody != null)
                myRigidBody.linearVelocity = Vector2.zero;
        }
    }

    public void RespawnNow()
    {
        transform.position = respawnPoint;

        var rb = GetComponentInParent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    /*
    /// <summary>
    /// Is called once per frame.
    /// </summary>
    void Update()
    {
        
    }
    */
}
