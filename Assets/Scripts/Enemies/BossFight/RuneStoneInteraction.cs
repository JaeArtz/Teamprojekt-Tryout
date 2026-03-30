using UnityEngine;
using System.Collections;

public class RuneStoneInteraction : TriggerAction
{
    [Header("Rune Visuals")]
    public GameObject sleepingRuneVisual;
    public GameObject wakingRuneVisual;
    public GameObject interactionHint;

    [Header("Boss Cleanup Settings")]
    [Tooltip("Das aktive Boss-Objekt (der animierte Golem)")]
    public GameObject activeBossGolem;
    [Tooltip("Das Objekt mit den statischen Trümmerteilen auf dem Boden")]
    public GameObject golemDebris;
    [Tooltip("Der Sound vom zerfallenden Gestein")]
    public AudioSource crumbleSound;

    [Header("Settings")]
    public AudioSource successSound;
    private bool isPlayerInside = false;
    private bool canInteract = false;

    public void EnableInteraction()
    {
        canInteract = true;
    }

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        if (!canInteract) yield break;

        while (!Input.GetKeyDown(KeyCode.E))
        {
            if (!isPlayerInside) yield break;
            yield return null;
        }

        // --- 1. SIEG-SEQUENZ START ---
        canInteract = false;
        if (interactionHint != null) interactionHint.SetActive(false);

        // 2. Visueller Runen-Tausch am Stein
        if (successSound != null) successSound.Play();
        if (sleepingRuneVisual != null) sleepingRuneVisual.SetActive(true);
        if (wakingRuneVisual != null) wakingRuneVisual.SetActive(false);

        // 3. Der Boss zerfällt (Akustisch)
        if (crumbleSound != null) crumbleSound.Play();

        // 4. Logik-Pause: Wir lassen den Sound kurz wirken
        yield return new WaitForSeconds(1.5f);

        // 5. Den aktiven Golem "verschwinden" lassen
        if (activeBossGolem != null) activeBossGolem.SetActive(false);

        // 6. Die statischen Steinhaufen auf dem Boden aktivieren
        if (golemDebris != null) golemDebris.SetActive(true);

        Debug.Log("Der Wächter wurde gebändigt und ist in Einzelteile zerfallen.");

        // Hier könnte man noch ein Victory-Theme oder ein Screen-Fade einbauen
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canInteract) return;
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            if (interactionHint != null) interactionHint.SetActive(true);
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