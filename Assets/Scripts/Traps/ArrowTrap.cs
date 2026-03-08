using UnityEngine;
public class ArrowTrap : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float arrowRange = 10f;
    [SerializeField] private float arrowSpeed = 10f;
    [SerializeField] private bool activateOnStart = false;
    
    private float cooldownTimer;
    private bool isActive = false;

    private void Start()
    {
        isActive = activateOnStart;
    }

    public void Activate()
    {
        isActive = true;
    }

    private void Attack()
    {
        cooldownTimer = 0;

        EnemyProjectile arrow = ProjectilePool.Instance.GetArrow();
        arrow.transform.position = firePoint.position;
        arrow.setRange(arrowRange);
        arrow.setSpeed(arrowSpeed);
        arrow.ActivateProjectile();
    }

    private void Update()
    {
        if(!isActive) return;
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= attackCooldown)
            Attack();
    }

    private void OnDrawGizmos()
    {
        if (firePoint == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(firePoint.position, firePoint.position + Vector3.left * arrowRange);
    }
}