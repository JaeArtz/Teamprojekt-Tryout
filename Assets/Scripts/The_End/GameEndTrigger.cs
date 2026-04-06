using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameEndTrigger : MonoBehaviour
{
    [Header("Referenzen")]
    public AudioSource backgroundMusic;    // Die Musik, die ausfaden soll
    public AudioSource endEffectSound;     // Der Sound, der im Schwarzbild spielt
    public LevelLoaderScript levelLoader;  // Dein Objekt für die Schwarzblende

    [Header("Einstellungen")]
    public float delayBeforeFade = 2.0f;   // Wie lange soll man den liegenden Player sehen?
    public string nextSceneName = "Credits";

    // Diese Funktion wird im zweiten Slot deines "On Interact ()" aufgerufen!
    public void StartFinalSequence()
    {
        StartCoroutine(EndSequenceRoutine());
    }

    private IEnumerator EndSequenceRoutine()
    {
        // 1. Musik ausfaden
        if (backgroundMusic != null)
        {
            float startVol = backgroundMusic.volume;
            for (float t = 0; t < 2f; t += Time.deltaTime)
            {
                backgroundMusic.volume = Mathf.Lerp(startVol, 0, t / 2f);
                yield return null;
            }
            backgroundMusic.Stop();
        }

        // 2. Warten (Zeit für die Liege-Animation)
        yield return new WaitForSeconds(delayBeforeFade);

        // 3. Schwarzblende (Fade-Out)
        if (levelLoader != null)
        {
            levelLoader.transition.SetTrigger("Start");
            yield return new WaitForSeconds(levelLoader.transitionTime);

            // 4. Sound im Dunkeln abspielen
            if (endEffectSound != null)
            {
                endEffectSound.Play();
                // Warten, bis der Sound fertig ist
                yield return new WaitForSeconds(endEffectSound.clip.length);
            }

            // 5. Szenenwechsel
            SceneManager.LoadScene(nextSceneName);
        }
    }
}