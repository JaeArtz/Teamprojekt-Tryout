using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelSelection : MonoBehaviour
{
    private GameObject myLevelLoader;

    public void Awake()
    {
        myLevelLoader = GameObject.Find("LevelLoader");
    }
    public void OpenLevel(int levelID)
    {
        myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene(SceneManager.GetActiveScene().buildIndex + levelID);
    }
}
