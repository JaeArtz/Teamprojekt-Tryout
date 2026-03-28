using UnityEngine;

public class EnemyDamage2 : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] protected int damage = 1;
    [SerializeField] private float hitCooldown = 1.0f; // cooldown until next possible damage intake

    [Header("Dodge Settings")]
    [SerializeField] private bool canBeDodged = true;

    private float lastHitTime = -99f; // = last time hit
    private bool isInPlayer = false;

    private PlayerHealth playerHealth;
    private IDodgeable dodgeableEntity;

    private void FixedUpdate()
    {
        // Is player in contact with a damage source, AND is the cooldown "ready"?
        if (isInPlayer && (Time.time - lastHitTime >= hitCooldown))
        {
            if (playerHealth != null)
            {
                TryApplyDamage();
            }
        }
    }

    private void TryApplyDamage()
    {        
        if (canBeDodged && dodgeableEntity != null && dodgeableEntity.IsInvulnerable)
        {            
            return;
        }
        
        playerHealth.TakeDamage(damage);

        // starts new Cooldown
        lastHitTime = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponentInParent<PlayerHealth>();
            dodgeableEntity = other.GetComponentInParent<IDodgeable>();

            if (playerHealth != null)
            {
                isInPlayer = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInPlayer = false;
            playerHealth = null;
            dodgeableEntity = null;
        }
    }
}