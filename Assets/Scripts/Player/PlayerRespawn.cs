using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 respawnPoint;

    
    /// <summary>
    /// Is called once before the first execution of Update after the MonoBehaviour is created.
    /// </summary>
    void Start()
    {
        respawnPoint = transform.position;
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

            var myRigidBody = GetComponent<Rigidbody2D>();
            if (myRigidBody != null)
                myRigidBody.linearVelocity = Vector2.zero;
        }
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
