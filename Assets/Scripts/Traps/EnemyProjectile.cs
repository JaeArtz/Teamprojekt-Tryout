using UnityEngine;

public class EnemyProjectile : EnemyDamage
{
    private float speed;
    private float range;
    private Vector3 startPosition;
    
    public void setRange(float range)
    {
        this.range = range;
    }

    public void setSpeed(float speed)
    {
        this.speed = speed;
    }
    public void ActivateProjectile()
    {
        startPosition = transform.position;
        gameObject.SetActive(true);
    }

    private void ReturnToPool()
    {
        ProjectilePool.Instance.ReturnArrowToPool(this);
    }

    private void Update()
    {
        float movementSpeed = speed * Time.deltaTime;
        transform.Translate(-movementSpeed, 0, 0, Space.World);
        
        if (Vector3.Distance(startPosition, transform.position) >= range)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerStay2D(other);
        if (other.CompareTag("Wall") || other.CompareTag("Ground") || other.CompareTag("Player") || other.CompareTag("PlayerProjectile"))
        {
            ReturnToPool();
        }
    }
}