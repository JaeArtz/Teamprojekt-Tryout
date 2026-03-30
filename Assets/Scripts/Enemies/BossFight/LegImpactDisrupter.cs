using UnityEngine;

public class LegImpactDisrupter : MonoBehaviour
{
    [Header("Schaden")]
    [SerializeField] private float damageToApply = 6f;
    [SerializeField] private float velocityThreshold = 2f;

    [Header("Sound Effekte")]
    [SerializeField] private AudioSource bumpSource;   // Für normales Dagegenlaufen/Abprallen
    [SerializeField] private AudioSource rumbleSource; // Für den harten Einschlag (Schaden)

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            float impactSpeed = collision.relativeVelocity.magnitude;

            if (impactSpeed > velocityThreshold)
            {
                // --- SCHADENS-FALL (Rumpeln) ---
                collision.gameObject.SendMessage("ApplyDamage", damageToApply, SendMessageOptions.DontRequireReceiver);

                if (rumbleSource != null) rumbleSource.PlayOneShot(rumbleSource.clip);
            }
            else
            {
                // --- NUR KONTAKT (Bump) ---
                // Spielt den Bump-Sound nur, wenn man nicht gerade stirbt
                if (bumpSource != null) bumpSource.PlayOneShot(bumpSource.clip);
            }

            // Stoppt das Rollen physisch
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null) playerRb.linearVelocity = Vector2.zero;
        }
    }
}