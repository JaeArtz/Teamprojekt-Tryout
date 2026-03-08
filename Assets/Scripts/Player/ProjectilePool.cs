using UnityEngine;
using System.Collections.Generic;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance;

    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private int poolSize = 10;

    [SerializeField] private EnemyProjectile arrowPrefab;
    [SerializeField] private int arrowPoolSize = 10; 

    private Queue<Projectile> pool = new Queue<Projectile>();
    private Queue<EnemyProjectile> arrowPool = new Queue<EnemyProjectile>();

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
        {
            Projectile proj = Instantiate(projectilePrefab, transform);
            proj.gameObject.SetActive(false);
            pool.Enqueue(proj);
        }

        for (int i = 0; i < arrowPoolSize; i++)
        {
            EnemyProjectile arrow = Instantiate(arrowPrefab, transform);
            arrow.gameObject.SetActive(false);
            arrowPool.Enqueue(arrow);
        }
    }

    public Projectile GetProjectile()
    {
        Projectile proj;

        if (pool.Count > 0)
            proj = pool.Dequeue();
        else
            proj = Instantiate(projectilePrefab, transform);

        proj.ResetProjectile();
        proj.gameObject.SetActive(true);
        return proj;
    }

    public void ReturnToPool(Projectile proj)
    {
        proj.gameObject.SetActive(false);
        pool.Enqueue(proj);
    }

    public EnemyProjectile GetArrow()
    {
        EnemyProjectile arrow;

        if (arrowPool.Count > 0)
            arrow = arrowPool.Dequeue();
        else
            arrow = Instantiate(arrowPrefab, transform);

        return arrow;
    }

    public void ReturnArrowToPool(EnemyProjectile arrow)
    {
        arrow.gameObject.SetActive(false);
        arrowPool.Enqueue(arrow);
    }
}