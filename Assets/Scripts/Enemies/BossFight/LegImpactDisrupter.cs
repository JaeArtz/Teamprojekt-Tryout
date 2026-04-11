using UnityEngine;

public class LegImpactDisrupter : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float damageToApply = 6f;
    [SerializeField] private float velocityThreshold = 2f;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource bumpSource;   // For FistBump
    [SerializeField] private AudioSource rumbleSource; // For FootStomp

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            float impactSpeed = collision.relativeVelocity.magnitude;

            if (impactSpeed > velocityThreshold)
            {
                // --- FallingFoot Damage Impact (Rumble) ---
                collision.gameObject.SendMessage("ApplyDamage", damageToApply, SendMessageOptions.DontRequireReceiver);

                if (rumbleSource != null) rumbleSource.PlayOneShot(rumbleSource.clip);
            }
            else // this might have to go, probably don't need it anymore
            {
                // --- On Contact (Bump) ---
                // Plays Bump once
                if (bumpSource != null) bumpSource.PlayOneShot(bumpSource.clip);
            }

            // stops rolling physically, can't pass through
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null) playerRb.linearVelocity = Vector2.zero;
        }
    }
}