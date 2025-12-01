using UnityEngine;
using System.Collections.Generic;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance;

    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<Projectile> pool = new Queue<Projectile>();

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
        {
            Projectile proj = Instantiate(projectilePrefab, transform);
            proj.gameObject.SetActive(false);
            pool.Enqueue(proj);
        }
    }

    public Projectile GetProjectile()
    {
        Projectile proj;

        if (pool.Count > 0)
        {
            proj = pool.Dequeue();
        }
        else
        {
            proj = Instantiate(projectilePrefab, transform);
        }

        // Reset bevor es sichtbar wird
        proj.ResetProjectile();

        proj.gameObject.SetActive(true);
        //Debug.Log("Projectile scale: " + proj.transform.localScale);
        return proj;
    }      


    public void ReturnToPool(Projectile proj)
    {
        proj.gameObject.SetActive(false);
        pool.Enqueue(proj);
    }
}
