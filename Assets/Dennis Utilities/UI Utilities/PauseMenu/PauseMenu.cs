using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    [SerializeField]
    public GameObject pauseMenuUI;

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
        hudUI.SetActive(true);
        Time.timeScale = 1.0f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        hudUI.SetActive(false);
        Time.timeScale = 0.0f;
        GameIsPaused = true;
    }

    public void ResetLevel()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void LoadMenu()
    {
        Time.timeScale = 1.0f;
        Debug.Log("Loading Menu...");
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
