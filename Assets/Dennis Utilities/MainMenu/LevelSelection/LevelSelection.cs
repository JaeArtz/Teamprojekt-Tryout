using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
public class LevelSelection : MonoBehaviour
{
    private GameObject myLevelLoader;

    public Button[] myButtons;
    public void Awake()
    {
        myLevelLoader = GameObject.Find("LevelLoader");

        for (int i = 0; i < myButtons.Length; i++)
        {
            myButtons[i].interactable = false;
        }
        if (SaveSystem.LoadLevelData() != null)
        {
            HashSet<int> unlockedLevels = SaveSystem.LoadLevelData();
            foreach (int level in unlockedLevels)
            {
                myButtons[level].interactable = true;
            }
        }
        else
        {
            myButtons[0].interactable = true;
        }
    }
    public void OpenLevel(int levelID)
    {
        myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene(SceneManager.GetActiveScene().buildIndex + levelID);
    }
}
