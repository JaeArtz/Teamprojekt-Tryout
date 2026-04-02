using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameEndTrigger : TriggerAction
{
    [Header("UI & Interaction")]
    [SerializeField] private GameObject interactionHint;

    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator endObjectAnimator;
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource endEffectSound;
    [SerializeField] private LevelLoaderScript levelLoader;

    [Header("Settings")]
    [SerializeField] private string nextSceneName = "Credits";
    [SerializeField] private float musicFadeDuration = 2.0f;
    [SerializeField] private string animationTriggerName = "StartEnd";

    private bool isPlayerInside = false;
    private bool sequenceStarted = false;

    private void Update()
    {
        // Startet die Sequenz bei E-Taste im Radius
        if (isPlayerInside && !sequenceStarted && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(StartEndSequence());
        }
    }

    private IEnumerator StartEndSequence()
    {
        sequenceStarted = true;

        // Vorbereitung: UI aus & Player einfrieren
        if (interactionHint != null) interactionHint.SetActive(false);
        if (playerController != null)
        {
            playerController.SetInputLocked(true);
            playerController.ResetHorizontalInputAndVelocity();
        }

        // --- 1) Eine Animation wird gespielt ---
        if (endObjectAnimator != null)
        {
            endObjectAnimator.SetTrigger(animationTriggerName);
            // Kleiner Wait, damit die Animation sichtbar startet
            yield return new WaitForSeconds(0.5f);
        }

        // --- 2) Der HintergrundSound stoppt/ausfadet ---
        if (backgroundMusic != null)
        {
            float startVol = backgroundMusic.volume;
            float elapsed = 0;
            while (elapsed < musicFadeDuration)
            {
                elapsed += Time.deltaTime;
                backgroundMusic.volume = Mathf.Lerp(startVol, 0, elapsed / musicFadeDuration);
                yield return null;
            }
            backgroundMusic.Stop();
        }

        // --- 3) Es soll dunkel werden (Bildschirm schwarz, Szene aktiv) ---
        if (levelLoader != null)
        {
            // Triggert den Fade-In deines LevelLoaders
            levelLoader.transition.SetTrigger("Start");

            // Warten, bis der Bildschirm durch den Crossfade komplett schwarz ist
            yield return new WaitForSeconds(levelLoader.transitionTime);

            // --- 4) Ein Sound wird abgespielt (Szene ist schwarz, aber noch aktiv) ---
            if (endEffectSound != null)
            {
                endEffectSound.Play();
                // Warten, bis der Sound fertig ist, bevor der Szenenwechsel den Ton killt
                yield return new WaitForSeconds(endEffectSound.clip.length);
            }

            // --- 5) Überleitung zur nächsten Szene (Abspann) ---
            SceneManager.LoadScene(nextSceneName);
        }
    }

    // --- Trigger Bereichs-Logik ---

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (sequenceStarted) return;
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

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        // Bleibt leer, da wir Update() für die E-Taste nutzen
        yield break;
    }
}