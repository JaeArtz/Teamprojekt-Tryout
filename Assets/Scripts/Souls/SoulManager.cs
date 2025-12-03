using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SoulManager : MonoBehaviour
{
    public static SoulManager Instance { get; private set; }

    private const string PREF_KEY = "CollectedSouls";
    public bool resetSouls = false; // optional: clear all on start (for testing)

    // Collected IDs
    private HashSet<string> collected = new HashSet<string>();

    private void Awake()
    {   
        if(resetSouls == true){
            ClearAll(); // optional: clear all on start (for testing)
        }
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(transform.root.gameObject);

        Load();
    }

    // Für Debug/Editor
    public IEnumerable<string> GetCollected() => collected.ToList();

    public bool HasSoul(string id) => collected.Contains(id);

    public void AddSoul(string id)
    {
        if (string.IsNullOrEmpty(id)) return;
        if (collected.Add(id))
        {
            Save();
            Debug.Log($"Soul collected: {id}");
        }
    }

    public void RemoveSoul(string id)
    {
        if (collected.Remove(id)) Save();
    }

    private void Save()
    {
        var serial = new Serialization<string>(collected);
        string json = JsonUtility.ToJson(serial);
        PlayerPrefs.SetString(PREF_KEY, json);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        if (!PlayerPrefs.HasKey(PREF_KEY)) return;
        string json = PlayerPrefs.GetString(PREF_KEY);
        var serial = JsonUtility.FromJson<Serialization<string>>(json);
        collected = new HashSet<string>(serial.ToList());
    }

    // optional: clear all (for testing)
    public void ClearAll()
    {
        collected.Clear();
        PlayerPrefs.DeleteKey(PREF_KEY);
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
