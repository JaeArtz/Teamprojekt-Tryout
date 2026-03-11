using System.Collections.Generic;
using UnityEngine;

public class CollectableManager : MonoBehaviour
{
    public static CollectableManager Instance { get; private set; }

    private HashSet<int> collectedLeaves = new HashSet<int>();
    public bool resetCollectables = false; // zum testen immer resetten

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(transform.root.gameObject);

        if(resetCollectables == true){
            ResetCollected(); // zum testen immer resetten
        }
        
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

