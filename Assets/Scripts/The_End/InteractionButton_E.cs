using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class InteractionButton_E : MonoBehaviour
{
    [Header("Visuals")]
    public GameObject interactionHint; // "E"-Button Popup

    [Header("Event")]
    [Tooltip("What happens when we press E?")]
    public UnityEvent onInteract;

    private bool playerIsInside = false;
    private bool playerHasInteracted = false;

    [Header("Settings")]
    public bool canInteractMultipleTimes = false;

    private void Awake()
    {       
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        // Hides Popup in the Beginning
        if (interactionHint != null) interactionHint.SetActive(false);
    }

    void Update()
    {        
        if (playerIsInside && (!playerHasInteracted || canInteractMultipleTimes))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }
    }

    private void Interact()
    {
        playerHasInteracted = true;

        // Hide Popup immediately
        if (interactionHint != null) interactionHint.SetActive(false);

        // Do whatever the Inspector says
        onInteract?.Invoke();

        Debug.Log("Interaktion ausgeführt!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!playerHasInteracted || canInteractMultipleTimes)
            {
                playerIsInside = true;
                if (interactionHint != null) interactionHint.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInside = false;
            if (interactionHint != null) interactionHint.SetActive(false);
        }
    }
}