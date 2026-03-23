using System.Collections;
using UnityEditor.EditorTools;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerController playerController;
    private SoulManager soulManager;
    private Rigidbody2D rb;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 0.7f;
    [SerializeField] private Transform firePoint;
    
    private float lastAttackTime = -Mathf.Infinity;
    private bool lightShotUnlocked = false;

    // Showcase vom LightShot
    private bool showcaseLightShot = false;

    private void Start()
    {
        if (soulManager.HasSoul("lightShotSoul"))
        {
            lightShotUnlocked = true;
        }
    }

    private void Awake()
    {
        soulManager = GameObject.Find("GameManager").GetComponent<SoulManager>();
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Lock input während Showcase
        if (playerController.IsInputLocked())
        {
            playerController.ResetHorizontalInputAndVelocity();
            return;
        }

        // Showcase triggern
        if (showcaseLightShot && playerController.Grounded)
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

        return true;
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
        playerController.SetInputLocked(true);

        yield return new WaitForSeconds(0.3f);
        Attack();
        yield return new WaitForSeconds(2f);

        playerController.SetInputLocked(false);
    }

    // Neue Methode für erweiterte Angriffsbedingungen
    public bool CanAttackInCurrentState()
    {
        if (!lightShotUnlocked)
            return false;
            
        if (playerController.IsInputLocked())
            return false;
            
        return CanAttack();
    }
}