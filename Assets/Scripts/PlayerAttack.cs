using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerMovement playerMovement;

    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] lightBalls; 
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && cooldownTimer > attackCooldown && playerMovement.canAttack())
        {
            Attack();
        }
    }

    private void Attack()
    {
        cooldownTimer = 0;
        //Object pooling for Lightball
        lightBalls[FindFireball()].transform.position = firePoint.position;
        lightBalls[FindFireball()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private int FindFireball()
    {
        for (int i = 0; i < lightBalls.Length; i++)
        {
            if (!lightBalls[i].activeInHierarchy)
                return i;
        }
        return 0; //all are active
    }
}
