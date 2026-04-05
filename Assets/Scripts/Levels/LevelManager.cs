using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private HashSet<int> finishedLevels = new HashSet<int>();
    private GameObject player;

    private void Awake()
    {
        player = GameObject.Find("Player");
        LoadLevelData();
    }

    private void LoadLevelData()
    {
        if (SaveSystem.LoadLevelData() == null)
        {
            SaveSystem.SaveLevelData(finishedLevels);
        }
        else
        {
            finishedLevels = SaveSystem.LoadLevelData();
        }
    }

    private void SaveLevelData()
    {
        SaveSystem.SaveData(player.GetComponent<PlayerHealth>());
        SaveSystem.SaveLevelData(finishedLevels);
    }

    public void SaveCurrentLevel()
    {
        finishedLevels.Add(SceneManager.GetActiveScene().buildIndex);
        SaveLevelData();
    }

}
