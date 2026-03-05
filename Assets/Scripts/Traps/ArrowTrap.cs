using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] arrows;
    [SerializeField] private float range;
    private float cooldownTimer;
    private bool isTriggered = false;
    private void Attak()
    {
        cooldownTimer = 0;

        arrows[FindArrow()].transform.position = firePoint.position;
        arrows[FindArrow()].transform.rotation = firePoint.rotation;
        arrows[FindArrow()].GetComponent<EnemyProjectile>().ActivateProjectileWithRange(range);
    }

    private int FindArrow()
    {
        for (int i = 0; i < arrows.Length; i++)
        {
            if (!arrows[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }
    private void Update()
    {
        if (!isTriggered)
        {
            RaycastHit2D hit = Physics2D.BoxCast(firePoint.position, new Vector2(0.1f, 1.0f), 0f, firePoint.right, range);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                isTriggered = true;
            }
        }

        if(isTriggered)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= attackCooldown)
            {
                Attak();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (firePoint == null) return;
        Gizmos.color = isTriggered ? Color.red : Color.cyan;
        Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.right * range);
    }
}
