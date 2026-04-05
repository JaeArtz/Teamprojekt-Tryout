using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class LevelSelection : MonoBehaviour
{
    private GameObject myLevelLoader;
    private LoadMenu myLoadMenu;
    public Button[] myButtons;
    public EventTrigger[] myTriggers;
    public void Awake()
    {
        myLevelLoader = GameObject.Find("LevelLoader");
        myLoadMenu = GameObject.Find("Canvas").GetComponent<LoadMenu>();
        for (int i = 0; i < myButtons.Length; i++)
        {
            myButtons[i].interactable = false;
            myTriggers[i].enabled = false;

        }
        if (SaveSystem.LoadLevelData() != null)
        {
            HashSet<int> unlockedLevels = SaveSystem.LoadLevelData();
            foreach (int level in unlockedLevels)
            {
                myButtons[level].interactable = true;
                myTriggers[level].enabled = true;
            }
        }
        else
        {
            myButtons[0].interactable = true;
            myTriggers[0].enabled = true;
        }
    }
    public void OpenLevel(int levelID)
    {
        myLoadMenu.PlayClickSound();
        myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene(SceneManager.GetActiveScene().buildIndex + levelID);
    }
}
