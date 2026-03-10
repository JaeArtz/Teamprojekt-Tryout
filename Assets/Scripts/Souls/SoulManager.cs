using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SoulManager : MonoBehaviour
{
    // Singleton-Instanz: Ermöglicht globalen Zugriff auf den SoulManager
    public static SoulManager Instance { get; private set; }

    private const string PREF_KEY = "CollectedSouls"; // Key für die Speicherung in PlayerPrefs
    public bool resetSouls = false; // reset aller Seelen fürs Testen in den Level

    // HAshSet mit den IDs der Seelen
    private HashSet<string> collected = new HashSet<string>();

    private void Awake()
    {   
        if(resetSouls == true){
            ClearAll();
        }
        
        // Singleton-Logik: Verhindert, dass mehrere Manager gleichzeitig existieren
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Sorgt dafür, dass der Manager beim Szenenwechsel nicht gelöscht wird
        DontDestroyOnLoad(transform.root.gameObject);

        // Vorhandene Daten laden
        Load();
    }

    // Gibt eine Liste der gesammelten IDs zurück (hilfreich für UI-Anzeigen)
    public IEnumerable<string> GetCollected() => collected.ToList();

    // Prüft, ob eine bestimmte Fähigkeit freigeschaltet ist
    public bool HasSoul(string id) => collected.Contains(id);

    // Fügt eine neue Seele hinzu und speichert den Fortschritt sofort
    public void AddSoul(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        // .Add() gibt true zurück, wenn die ID neu war
        if (collected.Add(id))
        {
            Save();
            Debug.Log($"Soul collected: {id}");
        }
    }


    // Entfernt eine Seele und speichert den Fortschritt sofort
    public void RemoveSoul(string id)
    {
        if (collected.Remove(id)) Save();
    }

    private void Save()
    {
        // Da Unitys JsonUtility keine Listen/HashSets direkt speichern kann, 
        // nutzen wir die Hilfsklasse 'Serialization'
        var serial = new Serialization<string>(collected);
        string json = JsonUtility.ToJson(serial);
        PlayerPrefs.SetString(PREF_KEY, json); // Speichert den JSON-String auf der Festplatte
        PlayerPrefs.Save();
    }

    private void Load()
    {
        // Prüfen, ob überhaupt schon Daten existieren
        if (!PlayerPrefs.HasKey(PREF_KEY)) return;
        string json = PlayerPrefs.GetString(PREF_KEY);
        var serial = JsonUtility.FromJson<Serialization<string>>(json);

        // Konvertiert die geladene Liste zurück in ein HashSet
        collected = new HashSet<string>(serial.ToList());
    }

    // Löscht den kompletten Fortschritt
    public void ClearAll()
    {
        collected.Clear();
        PlayerPrefs.DeleteKey(PREF_KEY);
    }
}

// Hilfsklasse: Erlaubt es JsonUtility, Listen zu verarbeiten, 
// da JsonUtility ein "Root-Objekt" benötigt.
[System.Serializable]
public class Serialization<T>
{
    public List<T> target;
    public Serialization(IEnumerable<T> e) { target = new List<T>(e); }
    public List<T> ToList() => target;
}
