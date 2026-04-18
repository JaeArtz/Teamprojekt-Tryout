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

    // Bool um anzuzeigen, ob Player gerade in der Zone ist oder nicht
    private bool playerIsInZone = false;
    // Player soll "gespeichert" werden
    private Collider2D playerCollider;

    private void Awake()
    {
        soulManager = SoulManager.Instance;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        if(soulManager == null) soulManager = SoulManager.Instance;

        if (dialogBox != null) dialogBox.SetActive(false);

        if (interactionHint_E != null) interactionHint_E.SetActive(false);

        if ((soulManager != null) && (soulManager.HasSoul(soulData.soulID)))
        {
            if (transform.parent != null) transform.parent.gameObject.SetActive(false);
            else gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerIsInZone && !collected && Input.GetKeyDown(KeyCode.E))
        {
            StartPickupSequence();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        playerIsInZone = true;
        playerCollider = other;
        if (interactionHint_E != null) interactionHint_E.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInZone = false;
            if (interactionHint_E != null) interactionHint_E.SetActive(false);
        }
    }

    public void StartPickupSequence()
    {
        if (collected) return;

        if (pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

        if (GetComponent<SpriteRenderer>() != null) GetComponent<SpriteRenderer>().enabled = false;
        if (interactionHint_E != null) interactionHint_E.SetActive(false);

        CollectSoul();
    }

    private void CollectSoul()
    {
        if (collected) return;
        collected = true;

        // Add Soul to SoulManager, via ID
        soulManager.AddSoul(soulData.soulID);

        // Suche über die Wurzel (Root) des Players, und alle Kinder (wollte ganz sicher die richtige Stelle erwischen)
        // Das findet so die Skripte zum Aktivieren der Seele, egal wo sie in der Player-Hierarchie liegen.
        // Unity kann mit FindGameObjectWithTag "" nach dem Player Tag suchen
        GameObject playerObj = playerCollider != null ? playerCollider.gameObject : GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            // Wir springen zur obersten Ebene des Players (Root)
            Transform pRoot = playerObj.transform.root;

            // Suche PlayerController
            var pm = pRoot.GetComponentInChildren<PlayerController>();
            if (pm != null) pm.OnSoulCollected(soulData);

            // Suche PlayerAttack
            var pa = pRoot.GetComponentInChildren<PlayerAttack>();
            if (pa != null) pa.OnSoulCollected(soulData);

            // Suche PlayerLight
            var pl = pRoot.GetComponentInChildren<PlayerLight>();
            if (pl != null) pl.ActivateLight();

            // Suche PlayerRoll (Armadillo)
            var pr = pRoot.GetComponentInChildren<PlayerRoll>();
            if (pr != null && soulData.soulID == "armadilloSoul")
            {
                pr.CanRoll = true;
            }

            //  PlayerJump regeln => entweder bleibt 1fach-Jump, oder Upgrade zu DoubleJump
            var pj = pRoot.GetComponentInChildren<PlayerJump>();
            if (pj != null && soulData.soulID == "RabbitSoul")
            {
                pj.CanDoubleJump = true;
                pj.ResetDoubleJumps();
            }

            // Suche PlayerGlide (Bird)
            var pgl = pRoot.GetComponentInChildren<PlayerGlide>();
            if (pgl != null && soulData.soulID == "birdSoul")
            {
                pgl.IsGlideUnlocked = true;
            }

        }

        // Dialog anzeigen
        if (dialogBox != null)
        {
            dialogBox.SetActive(true);
            StartCoroutine(HideDialogAfterDelay(dialogDuration));
        }

        if (GetComponent<SpriteRenderer>() != null) GetComponent<SpriteRenderer>().enabled = false;
        if (interactionHint_E != null) interactionHint_E.SetActive(false);

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

        if (transform.parent != null) transform.parent.gameObject.SetActive(false);
        else gameObject.SetActive(false);
    }
}