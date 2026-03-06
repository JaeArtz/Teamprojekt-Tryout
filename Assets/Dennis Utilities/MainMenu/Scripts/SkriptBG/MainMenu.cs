using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject myLevelLoader;
    public void StartGame()
    {
        //load the game scene
        myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //Game Scene
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
            Debug.Log(e.ToString());
        }
    }

    public void DeleteData()
    {
        try
        {

        SaveSystem.DeleteData();
        }
        catch(System.Exception e)
        {
            Debug.Log(e.ToString());
        }
        //Debug.Log("Optionmenu is not Implemented yet...");
        Debug.Log("Data deleted");
    }
    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

}
