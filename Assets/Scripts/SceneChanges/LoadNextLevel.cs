using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextLevel : MonoBehaviour
{
    private GameObject myLevelLoader;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Awake()
    {
        myLevelLoader = GameObject.Find("LevelLoader");
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        
    }
}
