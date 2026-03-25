using UnityEngine;

public class BossWave : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private int damage = 1; // Halbes Herz
    private float direction;
    private float lifetime = 4f;

    public void Setup(float dir)
    {
        direction = dir;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            PlayerController pc = collision.GetComponentInParent<PlayerController>();

            
            if (pc != null && !pc.IsInputLocked())
            {
                // check isRolling
              

                PlayerHealth health = collision.GetComponentInParent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(damage);
               
                }
            }
        }
    }
}