using System.Collections.Generic;
using UnityEngine;

public class CollectableManager : MonoBehaviour
{
    public static CollectableManager Instance { get; private set; }

    private HashSet<int> collectedLeaves = new HashSet<int>();
    public bool resetCollectedBoll = false; // zum testen immer resetten

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if(resetCollectedBoll == true){
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
        // Als String speichern (z. B. "0,1,2,5")
        PlayerPrefs.SetString("CollectedLeaves", string.Join(",", collectedLeaves));
        PlayerPrefs.Save();
    }

    private void LoadCollectedLeaves()
    {
        collectedLeaves.Clear();
        string saved = PlayerPrefs.GetString("CollectedLeaves", "");
        if (string.IsNullOrEmpty(saved)) return;

        string[] ids = saved.Split(',');
        foreach (string id in ids)
        {
            if (int.TryParse(id, out int leafId))
                collectedLeaves.Add(leafId);
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

