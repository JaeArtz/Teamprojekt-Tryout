using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Rigidbody2D rb;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 0.7f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private bool allowAttackWhileWallSliding = false; // Standard: nur am Boden
    
    [Header("Recoil Settings")]
    [SerializeField] private bool enableRecoil = false;
    [SerializeField] private float recoilForceX = 2f;
    [SerializeField] private float recoilForceY = 1f;
    
    private float lastAttackTime = -Mathf.Infinity;
    private bool lightShotUnlocked = false;

    // Showcase vom LightShot
    private bool showcaseLightShot = false;

    private void Start()
    {
        if (SoulManager.Instance != null && SoulManager.Instance.HasSoul("lightShotSoul"))
        {
            lightShotUnlocked = true;
        }
    }

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Lock input während Showcase
        if (playerMovement.IsInputLocked())
        {
            playerMovement.ResetHorizontalInputAndVelocity();
            return;
        }

        // Showcase triggern
        if (showcaseLightShot && playerMovement.IsGrounded())
        {
            showcaseLightShot = false;
            StartCoroutine(LightShotShowcase());
        }

        if (lightShotUnlocked && Input.GetMouseButtonDown(0) && CanAttack())
        {
            Attack();
        }
    }

    private bool CanAttack()
    {
        // Prüfe Cooldown
        if ((Time.time - lastAttackTime) < attackCooldown)
            return false;
        
        // Prüfe ob Spieler angreifen kann
        if (!allowAttackWhileWallSliding)
        {
            // Original-Verhalten: Nur am Boden
            return playerMovement.CanAttack();
        }
        else
        {
            // Erweitertes Verhalten: Am Boden ODER am Wall Slide
            return playerMovement.CanAttack(true); // Übergibt allowWhileWallSliding = true
        }
    }

    private void Attack()
    {
        lastAttackTime = Time.time;

        Projectile proj = ProjectilePool.Instance.GetProjectile();
        if (proj == null)
        {
            Debug.LogWarning("ProjectilePool returned null — Poolgröße/Reset prüfen!");
            return;
        }

        proj.transform.position = firePoint.position;
        
        // Richtung basierend auf Spieler-Rotation
        float direction = Mathf.Sign(transform.localScale.x);
        proj.SetDirection(direction);
        
        // Optional: Rückstoß beim Schießen
        if (enableRecoil)
        {
            ApplyRecoil(direction);
        }
    }

    private void ApplyRecoil(float direction)
    {
        if (rb != null)
        {
            Vector2 recoilForce = new Vector2(-direction * recoilForceX, recoilForceY);
            rb.AddForce(recoilForce, ForceMode2D.Impulse);
        }
    }

    public void UnlockShooting()
    {
        lightShotUnlocked = true;
    }

    public void OnSoulCollected(SoulData soul)
    {
        if (soul == null)
            return;

        if (soul.soulID == "lightShotSoul")
        {
            lightShotUnlocked = true;
            showcaseLightShot = true;
        }
    }

    private IEnumerator LightShotShowcase()
    {
        playerMovement.SetInputLocked(true);

        yield return new WaitForSeconds(0.3f);
        Attack();
        yield return new WaitForSeconds(2f);

        playerMovement.SetInputLocked(false);
    }

    // Neue Methode für erweiterte Angriffsbedingungen
    public bool CanAttackInCurrentState()
    {
        if (!lightShotUnlocked)
            return false;
            
        if (playerMovement.IsInputLocked())
            return false;
            
        return CanAttack();
    }
}