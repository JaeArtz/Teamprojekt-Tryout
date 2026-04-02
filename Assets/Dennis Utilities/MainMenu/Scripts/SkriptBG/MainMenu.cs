using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject myLevelLoader;
    [SerializeField] private Notify loadingFailed;
    [SerializeField] private Notify deletingFailed;
    [SerializeField] private Notify deletingSucceded;

    public GameObject quitUI;
    public GameObject mainUI;
    public GameObject newGameUI;
    public void StartGame()
    {        
        myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }    
}
