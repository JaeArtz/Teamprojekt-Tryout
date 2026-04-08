using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject myLevelLoader;

    public GameObject mainUI;
    
    private AudioSource m_audioComponent;

    [SerializeField]
    private AudioClip m_soundEffectHover;

    [SerializeField]
    private AudioClip m_soundEffectClick;

    [SerializeField]
    private AudioClip m_blank;
    private void Awake()
    {
        m_audioComponent = GetComponent<AudioSource>();
    }
    public void StartGame()
    {
        //TODO: waitForMillis instead of null value.
        m_soundEffectHover = m_blank;
        myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitGame()
    {
        //TODO: waitForMillis instead of null value.
        m_soundEffectHover = m_blank;
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    public void PlayHoverSound()
    {
        m_audioComponent.PlayOneShot(m_soundEffectHover);
    }

    public void PlayClickSound()
    {
        m_audioComponent.PlayOneShot(m_soundEffectClick);

        //TODO: waitForMillis instead of null value.
        m_soundEffectClick = m_blank;
    }
}
