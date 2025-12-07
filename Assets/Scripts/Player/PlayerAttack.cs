using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    private PlayerMovement playerMovement;

    [SerializeField] private float attackCooldown = 0.7f;
    [SerializeField] private Transform firePoint;
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
        if (showcaseLightShot && playerMovement.isGrounded())
        {
            showcaseLightShot = false;
            StartCoroutine(lightShotShowcase());
        }

        if (lightShotUnlocked && Input.GetMouseButtonDown(0) && CanAttack())
        {
            Attack();
        }
    }


    private bool CanAttack()
    {
        return (Time.time - lastAttackTime) >= attackCooldown && playerMovement.canAttack();
    }

    private void Attack()
    {
        lastAttackTime = Time.time;

        Projectile proj = ProjectilePool.Instance.GetProjectile();
        if (proj == null)
        {
            // Pool leer oder Fehler — präzise Logmeldung hilft beim Debuggen
            Debug.LogWarning("ProjectilePool returned null — Poolgröße/Reset prüfen!");
            return;
        }

        proj.transform.position = firePoint.position;
        proj.SetDirection(Mathf.Sign(transform.localScale.x));
    }

    public void unlockShooting()
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

    private IEnumerator lightShotShowcase()
    {
        playerMovement.SetInmputLocked(true);

        yield return new WaitForSeconds(0.3f);
        Attack();
        yield return new WaitForSeconds(2f);

        playerMovement.SetInmputLocked(false);
    }
}
