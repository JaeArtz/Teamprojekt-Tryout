using System.Collections.Generic;
using UnityEngine;

public class CollectableManager : MonoBehaviour
{

    private HashSet<int> collectedLeaves = new HashSet<int>();
    public bool resetCollectables = false; // zum testen immer resetten
    private GameObject player;
    private void Awake()
    {
        player = GameObject.Find("Player");

        LoadCollectedLeaves();
    }

    public void CollectLeaf(int id)
    {
        collectedLeaves.Add(id);
        SaveCollectedLeaves();
    }

    public bool IsLeafCollected(int id)
    {
        return collectedLeaves.Contains(id);
    }

    private void SaveCollectedLeaves()
    {
        SaveSystem.SaveLeafData(collectedLeaves);
        player.GetComponent<PlayerHealth>().UpdateLives();
    }

    private void LoadCollectedLeaves()
    {
        if (SaveSystem.LoadLeafData() == null)
        {
            SaveSystem.SaveLeafData(collectedLeaves);
        }
        else
        {
            collectedLeaves = SaveSystem.LoadLeafData();
        }
    }

    public int GetCollectedCount()
    {
        return collectedLeaves.Count;
    }

    public void ResetCollected()
    {
        collectedLeaves.Clear();
        PlayerPrefs.DeleteKey("CollectedLeaves");
    }
}

