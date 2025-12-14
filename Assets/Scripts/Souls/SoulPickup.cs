using UnityEngine;

public class SoulPickup : MonoBehaviour
{
    [Tooltip("Ziehen: das ScriptableObject der Seele")]
    public SoulData soulData;

    [Tooltip("Optional: UI-Dialog-Panel, welches aktiviert wird")]
    public GameObject dialogBox;
    public float dialogDuration = 3f;

    private bool collected = false;

    private void Start()
    {
        if (dialogBox != null) dialogBox.SetActive(false);

        // Wenn der Spieler die Seele schon hat, verstecke das Pickup-Objekt
        if (SoulManager.Instance != null && SoulManager.Instance.HasSoul(soulData.soulID))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;
        collected = true;

        // Add to SoulManager
        SoulManager.Instance?.AddSoul(soulData.soulID);

        var pm = other.GetComponentInParent<PlayerMovement>();
        if (pm != null)
        {
            pm.OnSoulCollected(soulData);
        }

        var pa = other.GetComponentInParent<PlayerAttack>();
        if (pa != null)
        {
            pa.OnSoulCollected(soulData);
        }

        // Dialog anzeigen (optional)
        if (dialogBox != null)
        {
            dialogBox.SetActive(true);
            StartCoroutine(HideDialogAfterDelay(dialogDuration));
        }

        // Spieleffekt oder Sound hier (optional)
        // Destroy oder deactivate object
        gameObject.SetActive(false);
    }

    private System.Collections.IEnumerator HideDialogAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (dialogBox != null) dialogBox.SetActive(false);
    }
}
