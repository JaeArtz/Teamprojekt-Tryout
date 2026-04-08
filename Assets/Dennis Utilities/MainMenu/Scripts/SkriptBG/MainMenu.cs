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
        
        PlayerData data = SaveSystem.LoadData();
        if(data == null)
        {
            myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            CreateNewGame();
        }


        

    }

    public void LoadGame()
    {
        try
        {
            PlayerData data = SaveSystem.LoadData();
            SaveSystem.AlterDataCheck(true);
            Debug.Log(data.currentScene);
            myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene(data.currentScene);
        }
        catch (System.Exception e)
        {
            if(NotifyManager.ManagerInstance != null)
            {
                NotifyManager.ManagerInstance.ShowNotification(loadingFailed);
            }
        }
    }

    public void DeleteData()
    {
        DeleteSaveFile();
    }
    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
    public void DeleteSaveFile()
    {
        quitUI.SetActive(true);
        mainUI.SetActive(false);
    }

    public void CreateNewGame()
    {
        newGameUI.SetActive(true);
        mainUI.SetActive(false);
    }
    public void Yes()
    {
        try
        {
            SaveSystem.DeleteData();
            NotifyManager.ManagerInstance.ShowNotification(deletingSucceded);
        }
        catch (System.Exception e)
        {
            NotifyManager.ManagerInstance.ShowNotification(deletingFailed);
        }
        No();
        Debug.Log("Data deleted...");
    }

    public void No()
    {
        mainUI.SetActive(true);
        quitUI.SetActive(false);
    }

    public void YesGame()
    {
        myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void NoGame()
    {
        mainUI.SetActive(true);
        newGameUI.SetActive(false);
    }
}
