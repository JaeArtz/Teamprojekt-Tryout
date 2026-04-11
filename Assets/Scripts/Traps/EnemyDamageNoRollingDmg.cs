using UnityEngine;

public class EnemyDamageNoRollingDmg : MonoBehaviour

{
    [Header("Damage Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageInterval = 2.0f;
    [SerializeField] private float initialDelay = 0f;

    private float nextDamageTime;
    private bool isInside = false;

    private PlayerHealth playerHealth;
    private PlayerRoll playerRoll;

    private void Update()
    {
        if (isInside && playerHealth != null)
        {            
            if (playerRoll == null)
            {
                playerRoll = playerHealth.GetComponentInChildren<PlayerRoll>();
            }

            // checks if Intervall is over
            if (Time.time >= nextDamageTime)
            {                
                // only take damage when NOT rolling
                bool isRolling = (playerRoll != null) && playerRoll.IsRolling;

                if (!isRolling)
                {
                    playerHealth.TakeDamage(damage);
                    nextDamageTime = Time.time + damageInterval;
                }
                else
                {
                    // Maybe cause damage instantly and ignore Delay, if Roll stops in Enemy
                    // optional, doesnt have to be like that
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {            
            playerHealth = collision.GetComponentInParent<PlayerHealth>();
            
            playerRoll = collision.GetComponentInParent<PlayerRoll>();
            if (playerRoll == null)
                playerRoll = collision.GetComponentInChildren<PlayerRoll>();

            if (playerHealth != null)
            {
                isInside = true;
                nextDamageTime = Time.time + initialDelay;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInside = false;
            nextDamageTime = 0;
            playerHealth = null;
            playerRoll = null;
        }
    }
}