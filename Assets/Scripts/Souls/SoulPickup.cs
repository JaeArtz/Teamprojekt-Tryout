using UnityEngine;

public class SoulPickup : MonoBehaviour
{
    [Tooltip("Ziehen: das ScriptableObject der Seele")]
    public SoulData soulData;

    [Tooltip("Optional: UI-Dialog-Panel, welches aktiviert wird")]
    public GameObject dialogBox;
    public float dialogDuration = 3f;

    [Tooltip("Slot für das Objekt mit dem E_Button")]
    public GameObject interactionHint_E;

    [Header("Audio")]
    [Tooltip("Der Sound, der beim Einsammeln einmalig spielt")]
    public AudioClip pickupSound;
    private AudioSource audioSource;

    private SoulManager soulManager;
    private bool collected = false;

    // Bool um anzuzeigen, on Player gerade in der Zone ist oder nicht-
    // relevant für E_Button
    private bool playerIsInZone = false;
    // Player soll "gespeichert" werden (dieser Player = other)
    private Collider2D playerCollider;

    private void Awake()
    {
        soulManager = GameObject.Find("GameManager").GetComponent<SoulManager>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        if (dialogBox != null) dialogBox.SetActive(false);

        // E-Button initial verstecken, wird dann sichtbar wenn Player den Collider betritt
        if (interactionHint_E != null) interactionHint_E.SetActive(false);

        // Wenn der Spieler die Seele schon hat, verstecke das Pickup-Objekt
        if (soulManager.HasSoul(soulData.soulID))
        {
            // Falls ein Parent-Container genutzt wird, diesen deaktivieren
            if (transform.parent != null) transform.parent.gameObject.SetActive(false);
            else gameObject.SetActive(false);
        }
    }

    // Solange der Spieler im Trigger ist, wird geprüft auf den Tastendruck "E" 
    private void Update()
    {
        // ACP: Änderung => Der SpeechBubbleManager auf dem Parent macht 
        // den Input-Check. 
        if (playerIsInZone && !collected && Input.GetKeyDown(KeyCode.E))
        {
            // Wenn wir manuell einsammeln, triggern wir die Sequenz
            StartPickupSequence();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        // ACP: Änderung => anstatt direkt einzusammeln, wird nur "vorbereitet" für Einsammeln
        playerIsInZone = true;
        playerCollider = other;
        if (interactionHint_E != null) interactionHint_E.SetActive(true);
    }

    //Deaktiviert die Interaktion, wenn der Spieler weggeht
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInZone = false;
            if (interactionHint_E != null) interactionHint_E.SetActive(false);
        }
    }

    // Diese Methode wird nun vom SpeechBubbleManager aufgerufen, 
    // wenn der Dialog startet, damit die Seele sofort optisch verschwindet.
    // (Der Text-Popup soll danach noch etwas sichtbar sein "ROLLING UNLOCKED" etc.)
    public void StartPickupSequence()
    {
        if (collected) return;

        if (pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

        // Seele optisch ausblenden
        if (GetComponent<SpriteRenderer>() != null) GetComponent<SpriteRenderer>().enabled = false;
        if (interactionHint_E != null) interactionHint_E.SetActive(false);

        CollectSoul();
    }

    //collected kommt in eine eigene Methode (=> E-Button)
    private void CollectSoul()
    {
        if (collected) return; // Doppeltes Sammeln verhindern
        collected = true;

        // Add to SoulManager
        soulManager.AddSoul(soulData.soulID);

        // Falls playerCollider null ist (z.B. durch Distanz-Check des Managers statt Trigger), 
        // suchen wir den Player kurz per Tag
        Collider2D targetCollider = playerCollider;
        if (targetCollider == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) targetCollider = playerObj.GetComponent<Collider2D>();
        }

        if (targetCollider != null)
        {
            // Suche das PlayerController Script auf dem Spieler
            var pm = targetCollider.GetComponentInParent<PlayerController>();
            if (pm != null)
            {
                pm.OnSoulCollected(soulData);
            }

            // Suche das PlayerAttack Script auf dem Spieler
            var pa = targetCollider.GetComponentInParent<PlayerAttack>();
            if (pa != null)
            {
                pa.OnSoulCollected(soulData);
            }

            // Suche das PlayerLight Script auf dem Spieler und aktiviere es
            var pl = targetCollider.GetComponentInParent<PlayerLight>();
            if (pl != null)
            {
                pl.ActivateLight();
            }

            // Suche das PlayerRoll Script auf dem Spieler und erlaube das Rollen, wenn es die Armadillo-Seele ist
            var pr = targetCollider.GetComponentInParent<PlayerRoll>();
            if (pr != null && soulData.soulID == "armadilloSoul")
            {
                pr.CanRoll = true;
            }

            // Suche das PlayerGlide Script auf dem Spieler
            var pgl = targetCollider.GetComponentInParent<PlayerGlide>();
            if (pgl != null && soulData.soulID == "birdSoul")
            {
                pgl.IsGlideUnlocked = true;
            }
        }

        // Dialog anzeigen (Nur Skillname)
        if (dialogBox != null)
        {
            dialogBox.SetActive(true);
            // Nach einer kurzen Zeit wird die Anzeige wieder deaktiviert
            StartCoroutine(HideDialogAfterDelay(dialogDuration));
        }

        // Schaltet die Visuals aus (damit das Script für den Dialog noch kurz weiterlaufen kann)
        // Wenn du kein SpriteRenderer nutzt, kannst du hier auch gameObject.SetActive(false) lassen.
        if (GetComponent<SpriteRenderer>() != null) GetComponent<SpriteRenderer>().enabled = false;
        if (interactionHint_E != null) interactionHint_E.SetActive(false);

        // Wenn kein Dialog genutzt wird, sofort ganz aus
        if (dialogBox == null)
        {
            if (transform.parent != null) transform.parent.gameObject.SetActive(false);
            else gameObject.SetActive(false);
        }
    }

    private System.Collections.IEnumerator HideDialogAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (dialogBox != null) dialogBox.SetActive(false);
        // Am Ende das gesamte Objekt deaktivieren
        // Und auch den Parent (SpeechBubbleManager) mit deaktivieren
        if (transform.parent != null) transform.parent.gameObject.SetActive(false);
        else gameObject.SetActive(false);
    }
}