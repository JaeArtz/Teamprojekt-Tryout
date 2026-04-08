using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class InteractionButton_E : MonoBehaviour
{
    [Header("Visuals")]
    public GameObject interactionHint; // Dein "E"-Button Popup

    [Header("Event")]
    [Tooltip("Was soll passieren, wenn E gedrückt wird?")]
    public UnityEvent onInteract;

    private bool isPlayerInside = false;
    private bool hasInteracted = false;

    [Header("Settings")]
    public bool canInteractMultipleTimes = false;

    private void Awake()
    {
        // Sicherstellen, dass der Collider auf Trigger steht
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        // Popup am Anfang verstecken
        if (interactionHint != null) interactionHint.SetActive(false);
    }

    void Update()
    {
        // Wenn Spieler drin, noch nicht interagiert (oder mehrfach erlaubt) und E drückt
        if (isPlayerInside && (!hasInteracted || canInteractMultipleTimes))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }
    }

    private void Interact()
    {
        hasInteracted = true;

        // Popup sofort ausblenden
        if (interactionHint != null) interactionHint.SetActive(false);

        // Das ausführen, was du im Inspector zugewiesen hast
        onInteract?.Invoke();

        Debug.Log("Interaktion ausgeführt!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!hasInteracted || canInteractMultipleTimes)
            {
                isPlayerInside = true;
                if (interactionHint != null) interactionHint.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            if (interactionHint != null) interactionHint.SetActive(false);
        }
    }
}