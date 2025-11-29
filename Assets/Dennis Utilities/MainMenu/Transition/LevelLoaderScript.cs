using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoaderScript : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1.0f;


    public void LoadNextLevel()
    {
        if(SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCount)
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    //TODO: Include this Method to others.

    IEnumerator LoadLevel(int levelIndex)
    {
        //Play animation
        transition.SetTrigger("Start");
        //wait
        yield return new WaitForSeconds(transitionTime);
        //Load Scene
        SceneManager.LoadScene(levelIndex);
    }
}
