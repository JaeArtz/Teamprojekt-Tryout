using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuDecider : MonoBehaviour
{
    private void Start()
    {
        try
        {
            if (SaveSystem.LoadSelectedFileData() == 0)
            {
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                SceneManager.LoadScene("LoadedMenu");
            }
        }
        catch(System.Exception error)
        {
            SaveSystem.SaveSelectedFileData(0);
            SceneManager.LoadScene("MainMenu");
        }
    }
}
