using UnityEngine;

public class SoulPickup : MonoBehaviour
{
    [Tooltip("Ziehen: das ScriptableObject der Seele")]
    public SoulData soulData;

    [Tooltip("Optional: UI-Dialog-Panel, welches aktiviert wird")]
    public GameObject dialogBox;
    public float dialogDuration = 3f;
    private SoulManager soulManager;
    private bool collected = false;

    private void Awake()
    {
        soulManager = GameObject.Find("GameManager").GetComponent<SoulManager>();
        
    }
    private void Start()
    {
        if (dialogBox != null) dialogBox.SetActive(false);

        // Wenn der Spieler die Seele schon hat, verstecke das Pickup-Objekt
        if (soulManager.HasSoul(soulData.soulID))
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
        soulManager.AddSoul(soulData.soulID);

        // Suche das PlayerController Script auf dem Spieler
        var pm = other.GetComponentInParent<PlayerController>();
        if (pm != null)
        {
            pm.OnSoulCollected(soulData);
        }

        // Suche das PlayerAttack Script auf dem Spieler
        var pa = other.GetComponentInParent<PlayerAttack>();
        if (pa != null)
        {
            pa.OnSoulCollected(soulData);
        }

        // Suche das PlayerLight Script auf dem Spieler und aktiviere es
        var pl = other.GetComponentInParent<PlayerLight>();
        if (pl != null)
        {
            pl.ActivateLight();
        }

        // Suche das PlayerRoll Script auf dem Spieler und erlaube das Rollen, wenn es die Armadillo-Seele ist
        var pr = other.GetComponentInParent<PlayerRoll>();
        if (pr != null && soulData.soulID == "armadilloSoul")
        {
            pr.CanRoll = true;
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
