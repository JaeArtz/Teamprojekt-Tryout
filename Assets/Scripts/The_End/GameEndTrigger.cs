using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameEndTrigger : MonoBehaviour
{
    [Header("References")]
    [Tooltip("This Sound will fade out at End of Level")]
    public AudioSource backgroundMusic;
    [Tooltip("Sound that will play in the Dark after Fadeout into Black (The Two Heartbeats)")]
    public AudioSource endEffectSound;
    [Tooltip("Insert LevelLoader here")]
    public LevelLoaderScript levelLoader;

    private GameObject myLevelLoader;
    private GameObject pauseCanvas;
    private LastBreath lastBreath;
    [Header("Settings")]
    [Tooltip("Duration in seconds, how long we can see the Player before Fade starts")]
    public float delayBeforeFade = 7.0f;

    public Animator animator;
    public GameObject AnimatorCanvas;
    public void Awake()
    {
        myLevelLoader = GameObject.Find("LevelLoader");
        pauseCanvas = GameObject.Find("PauseCanvas");
        lastBreath = GameObject.Find("LastSeconds").GetComponent<LastBreath>();

    }

    public void Start()
    {
        AnimatorCanvas.SetActive(false);
    }

    //Used in "On Interact ()"
    public void StartFinalSequence()
    {
        StartCoroutine(EndSequenceRoutine());
    }

    private IEnumerator EndSequenceRoutine()
    {
        pauseCanvas.SetActive(false);
        StartCoroutine(lastBreath.LastDyingBreath());
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
                AnimatorCanvas.SetActive(true);
                animator.Play("TreeAnim");
                yield return new WaitForSeconds(3);
                endEffectSound.Play();
                // Wait for Sound to be done
                yield return new WaitForSeconds((endEffectSound.clip.length - 7.0f));
            }

            // 5. SwitchScene
            myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}