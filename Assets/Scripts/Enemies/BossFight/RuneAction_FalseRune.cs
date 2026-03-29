using System.Collections;
using UnityEngine;

public class RuneAction_FalseRune : TriggerAction
{
    [Header("Puzzle References")]
    public RunePuzzleManager puzzleManager;

    [Header("Visual Effects (Feedback)")]
    public GameObject temporaryGlowObject;
    public GameObject temporaryLightObject;
    public float glowDuration = 1.5f;

    [Header("Audio Settings")]
    public AudioSource errorSoundSource;

    [Header("Interaction Settings")]
    public GameObject interactionHint;

    private bool isPlayerInside = false;

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        while (!Input.GetKeyDown(KeyCode.E))
        {
            if (!isPlayerInside) yield break;
            yield return null;
        }

        if (errorSoundSource != null) errorSoundSource.Play();
        if (interactionHint != null) interactionHint.SetActive(false);

        // Visual Effects on
        if (temporaryGlowObject != null) temporaryGlowObject.SetActive(true);
        if (temporaryLightObject != null) temporaryLightObject.SetActive(true);

        // Reset
        if (puzzleManager != null) puzzleManager.ResetPuzzle();

        yield return new WaitForSeconds(glowDuration);

        // Visual Effects off
        if (temporaryGlowObject != null) temporaryGlowObject.SetActive(false);
        if (temporaryLightObject != null) temporaryLightObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
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