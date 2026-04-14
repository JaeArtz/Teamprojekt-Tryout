using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameEndTrigger : MonoBehaviour
{
    [Header("References")]
    public AudioSource backgroundMusic;    // Die Musik, die ausfaden soll
    public AudioSource endEffectSound;     // Der Sound, der im Schwarzbild spielt
    public LevelLoaderScript levelLoader;  // Dein Objekt für die Schwarzblende

    private GameObject myLevelLoader;
    [Header("Settings")]
    [Tooltip("Duration in seconds, how long we can see the Player before Fade starts")]
    public float delayBeforeFade = 7.0f;

    public void Awake()
    {
        myLevelLoader = GameObject.Find("LevelLoader");
    }

    //Used in "On Interact ()"
    public void StartFinalSequence()
    {
        StartCoroutine(EndSequenceRoutine());
    }

    private IEnumerator EndSequenceRoutine()
    {
        // 1. Music- fade
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

        // 2. Waiting (for sleep-Animation), just before end
        yield return new WaitForSeconds(delayBeforeFade);

        // 3. Fade-Out, BlackScreen
        if (levelLoader != null)
        {
            levelLoader.transition.SetTrigger("Start");
            yield return new WaitForSeconds(levelLoader.transitionTime);

            // 4. Sound in the Dark (2 slow low HeartBeats)
            if (endEffectSound != null)
            {
                endEffectSound.Play();
                // Wait for Sound to be done
                yield return new WaitForSeconds(endEffectSound.clip.length);
            }

            // 5. SwitchScene
            myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}