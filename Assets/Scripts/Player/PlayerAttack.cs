using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerMovement playerMovement;

    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    private float cooldownTimer = Mathf.Infinity;
    private bool lightShotUnlocked = false;

    private void Start()
    {
        if(SoulManager.Instance != null && SoulManager.Instance.HasSoul("lightShotSoul"))
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
        cooldownTimer += Time.deltaTime;
        if (lightShotUnlocked && Input.GetMouseButtonDown(0) && cooldownTimer > attackCooldown && playerMovement.canAttack())
        {
            Attack();
        }
    }

    private void Attack()
    {
        cooldownTimer = 0;

        Projectile proj = ProjectilePool.Instance.GetProjectile();
        proj.transform.position = firePoint.position;
        proj.SetDirection(Mathf.Sign(transform.localScale.x));
    }


    public void unlockShooting()
    {
        lightShotUnlocked = true;
    }

    public void OnSoulCollected(SoulData soul)
    {
        if(soul == null)
            return;

        if(soul.soulID == "lightShotSoul")
        {
            lightShotUnlocked = true;
        }
    }
}
