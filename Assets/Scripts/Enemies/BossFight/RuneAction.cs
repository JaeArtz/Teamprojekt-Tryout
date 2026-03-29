using System.Collections;
using UnityEngine;

public class RuneAction : TriggerAction
{
    [Header("Rune Settings")]
    public string runeID;
    public RunePuzzleManager puzzleManager;

    [Header("Visual Effects (Nur Kinder!)")]
    public GameObject activatedVisuals;
    public GameObject lightPulseObject;
    public GameObject interactionHint;

    [Header("Continuous Effects")]
    public AudioSource hummingSound;

    private bool isActivated = false;
    private bool isPlayerInside = false;
    private Coroutine activeCoroutine; 

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        // If already activated, don't look any further
        if (isActivated) yield break;

        // Wait for E-Button
        while (!Input.GetKeyDown(KeyCode.E))
        {
            if (!isPlayerInside) yield break;
            yield return null;
        }

        // --- ACTIVATION ---
        isActivated = true;

        if (interactionHint != null) interactionHint.SetActive(false);
        if (activatedVisuals != null) activatedVisuals.SetActive(true);
        if (lightPulseObject != null) lightPulseObject.SetActive(true);
        if (hummingSound != null) hummingSound.Play();

        if (puzzleManager != null)
            puzzleManager.RegisterRuneActivation(runeID, this, ctx);
    }

    // Method os used by Manager to control Runes
    public void Deactivate()
    {
        isActivated = false;

        // for one specific Rune (current Rune)
        StopAllCoroutines();

        // only deactivates children (glow, light, E-Button and sound)
        if (activatedVisuals != null) activatedVisuals.SetActive(false);
        if (lightPulseObject != null) lightPulseObject.SetActive(false);
        if (hummingSound != null) hummingSound.Stop();
        if (interactionHint != null) interactionHint.SetActive(false);

        // HINWEIS: Hier steht KEIN SetActive(false) für das Hauptobjekt!
        Debug.Log($"Rune {runeID} zurückgesetzt.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated) return;
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