using UnityEngine;

public class BouncerFistTrigger : MonoBehaviour
{
    [Header("--- Setup ---")]
    [Tooltip("Drag your BossBouncerFist in here")]
    public BouncerFist bouncerFist;

    [Tooltip("Drag your Player in here")]
    public GameObject playerObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Who is the Player?
        bool isPlayer = (other.gameObject == playerObject || other.transform.parent?.gameObject == playerObject);

        if (isPlayer)
        {
            if (bouncerFist != null)
            {
                bouncerFist.TriggerAttack();
                Debug.Log("<color=yellow>TRIGGER: Player here! Hit is in Order.</color>");
            }
        }
        else
        {
            // Checking, if anything touches the Trigger
            Debug.Log("Trigger touched by: " + other.name);
        }
    }
}