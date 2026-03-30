using UnityEngine;
using System.Collections;

public class RuneStoneInteraction : TriggerAction
{
    [Header("Rune Visuals")]
    public GameObject sleepingRuneVisual;
    public GameObject wakingRuneVisual;
    public GameObject interactionHint;

    [Header("Boss Cleanup Settings")]
    [Tooltip("The active, awoken Golem in Fight Mode")]
    public GameObject activeBossGolem;
    [Tooltip("Boss-Golem Debris, after Sleeping Rune has been reactivated, a part of Debris is \"walkable\"")]
    public GameObject golemDebris;
    [Tooltip("Sound of Golem falling into Pieces, after SleepingRune has been reactivated")]
    public AudioSource crumbleSound;

    [Header("Settings")]
    public AudioSource successSound;
    private bool playerIsInside = false;
    private bool youCanInteract = false;

    public void EnableInteraction()
    {
        youCanInteract = true;
    }

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        if (!youCanInteract) yield break;

        while (!Input.GetKeyDown(KeyCode.E))
        {
            if (!playerIsInside) yield break;
            yield return null;
        }

        // --- 1. VICTORY_SEQUENCE START ---
        youCanInteract = false;
        if (interactionHint != null) interactionHint.SetActive(false);

        // 2. Visual Swapping of Rune
        if (successSound != null) successSound.Play();
        if (sleepingRuneVisual != null) sleepingRuneVisual.SetActive(true);
        if (wakingRuneVisual != null) wakingRuneVisual.SetActive(false);

        // 3. Boss crumbles
        if (crumbleSound != null) crumbleSound.Play();

        // 4. Let Sound do its thing for a moment
        yield return new WaitForSeconds(1.5f);

        // 5. Let active Golem disappear
        if (activeBossGolem != null) activeBossGolem.SetActive(false);

        // 6. "Activate" GolemDebris
        if (golemDebris != null) golemDebris.SetActive(true);

        Debug.Log("The Guardian has been put back to Sleep.");

       
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!youCanInteract) return;
        if (other.CompareTag("Player"))
        {
            playerIsInside = true;
            if (interactionHint != null) interactionHint.SetActive(true);
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