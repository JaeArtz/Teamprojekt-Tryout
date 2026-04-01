using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [Header("ShockWave SingleWave Settings")]
    [Tooltip("TravelSpeed of Wave")]
    public float speed = 8f;
    [Tooltip("How long WaveAnimation exists and goes on, until despawn")]
    public float lifetime = 1.5f;
    [Tooltip("Damage of Wave, is only dealt once to be nice")]
    public int damage = 1;          

    private int direction = 1;      // 1 = Right, -1 = Left
    private bool hasHitPlayer = false; // Safety bool, only want to hit Player once with each Wave

    public void Setup(int dir)
    {
        direction = dir;

        // mirrors Sprite depending on direction
        transform.localScale = new Vector3(dir, 1, 1);

        // destroys object, after lifetime ended
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // constant movement into one single direction, along x-direction, on floor height (Y-coordinate)
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // only deals damage if player has not been hit yet;
        // could be altered to always deal damage, but with appropriate cooldown
        if (other.CompareTag("Player") && !hasHitPlayer)
        {
            var health = other.GetComponentInParent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                hasHitPlayer = true; // this bars ONE wave that has already hit the player form hitting again
                Debug.Log("Shockwave HIt: One Time " + damage + " damage.");
            }
        }
    }
}