using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    private GameObject myLevelLoader;
    private GameObject mySaveGame;
    public static bool GameIsPaused = false;
    [SerializeField]
    public GameObject pauseMenuUI;
    private GameObject player;
    public GameObject hudUI;
    public GameObject quitUI;


    private AudioSource m_audioComponent;
    [SerializeField]
    private AudioClip m_soundEffectHover;
    [SerializeField]
    private AudioClip m_soundEffectClick;
    [SerializeField]
    private AudioClip m_blank;
    [SerializeField]
    private AudioClip m_backUp;
    private void Awake()
    {
        m_audioComponent = GetComponent<AudioSource>();
        player = GameObject.Find("Player");
        myLevelLoader = GameObject.Find("LevelLoader");
        mySaveGame = GameObject.Find("Saver");
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameIsPaused)
            {
                Resume();
               
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        quitUI.SetActive(false);
        player.GetComponent<PlayerAttack>().enabled = (true);
        hudUI.SetActive(true);
        Time.timeScale = 1.0f;
        GameIsPaused = false;
    }

    void Pause()
    {
        m_soundEffectHover = m_backUp;
        pauseMenuUI.SetActive(true);
        player.GetComponent<PlayerAttack>().enabled = (false);
        hudUI.SetActive(false);
        Time.timeScale = 0.0f;
        GameIsPaused = true;
    }

    public void ResetLevel()
    {
        m_soundEffectHover = m_blank;
        Time.timeScale = 1.0f;
        myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void LoadMenu()
    {
        m_soundEffectHover = m_blank;
        Time.timeScale = 1.0f;
        Debug.Log("Loading Menu...");
        mySaveGame.GetComponent<SaveGame>().SaveCurrentGame();
        myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene("LoadedMenu");
    }

    public void QuitGame()
    {
        quitUI.SetActive(true);
        pauseMenuUI.SetActive(false);
    }

    public void Yes()
    {
        m_soundEffectHover = m_blank;
        Quit();
    }

    public void No()
    {
        quitUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }
    public void Quit()
    {
        m_soundEffectHover = m_blank;
        mySaveGame.GetComponent<SaveGame>().SaveCurrentGame();
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

    public void ClickSoundPlayNoDeactivation()
    {
        m_audioComponent.PlayOneShot(m_soundEffectClick);
    }
}
