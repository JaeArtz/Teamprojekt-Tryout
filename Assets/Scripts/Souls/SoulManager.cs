using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SoulManager : MonoBehaviour
{

    public bool resetSouls = false; // optional: clear all on start (for testing)

    private GameObject player;
    // Collected IDs
    private HashSet<string> collected = new HashSet<string>();

    private void Awake()
    {
        player = GameObject.Find("Player");

        Load();
    }

    // Für Debug/Editor
    public IEnumerable<string> GetCollected() => collected.ToList();

    public bool HasSoul(string id) => collected.Contains(id);

    public  void AddSoul(string id)
    {
        if (string.IsNullOrEmpty(id)) return;
        
        if (collected.Add(id))
        {
            Save();
            SaveSystem.SaveData(player.GetComponent<PlayerHealth>());
            Debug.Log($"Soul collected: {id}");
        }
        
    }

    public  void RemoveSoul(string id)
    {
        if (collected.Remove(id)) Save();
    }

    private  void Save()
    {
        SaveSystem.SaveSoulData(collected);
        //var serial = new Serialization<string>(collected);
        //string json = JsonUtility.ToJson(serial);
        //PlayerPrefs.SetString(PREF_KEY, json);
        //PlayerPrefs.Save();
    }

    private  void Load()
    {
        if(SaveSystem.LoadSoulData() == null)
        {
            SaveSystem.SaveSoulData(collected);
        }
        else
        {
            collected = SaveSystem.LoadSoulData();
        }
    }

    // optional: clear all (for testing)
    public  void ClearAll()
    {
        collected.Clear();
        //PlayerPrefs.DeleteKey(PREF_KEY);
    }
}

// helper for serializing HashSet/List with JsonUtility
[System.Serializable]
public class Serialization<T>
{
    public List<T> target;
    public Serialization(IEnumerable<T> e) { target = new List<T>(e); }
    public List<T> ToList() => target;
}
