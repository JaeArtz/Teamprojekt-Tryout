using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        //load the game scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //Game Scene
    }

    public void LoadGame()
    {
        Debug.Log("Loading Game is not Implemented yet...");
    }

    public void OptionMenu()
    {
        Debug.Log("Optionmenu is not Implemented yet...");
    }
    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

}
