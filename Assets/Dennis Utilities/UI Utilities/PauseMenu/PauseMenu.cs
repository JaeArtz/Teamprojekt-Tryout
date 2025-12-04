using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public GameObject myLevelLoader;
    public static bool GameIsPaused = false;
    [SerializeField]
    public GameObject pauseMenuUI;
    public GameObject player;
    public GameObject hudUI;
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
        player.SetActive(true);
        hudUI.SetActive(true);
        Time.timeScale = 1.0f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        player.SetActive(false);
        hudUI.SetActive(false);
        Time.timeScale = 0.0f;
        GameIsPaused = true;
    }

    public void ResetLevel()
    {
        Time.timeScale = 1.0f;
        myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void LoadMenu()
    {
        Time.timeScale = 1.0f;
        Debug.Log("Loading Menu...");
        myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
