using UnityEngine;

public class EnemyProjectile : EnemyDamage
{
    [SerializeField] private float speed;
    private float resetTime;
    private float maxLifetime;

    private float lifetime;

    public void ActivateProjectileWithRange(float range)
    {
        lifetime = 0;
        maxLifetime = range / speed;
        gameObject.SetActive(true);
    }

    public void Update()
    {
        float movementSpeed = speed * Time.deltaTime;
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > Mathf.Abs(maxLifetime))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Player") || collision.CompareTag("Ground"))
        {
            base.OnTriggerStay2D(collision);
            gameObject.SetActive(false);
        }
    }
}