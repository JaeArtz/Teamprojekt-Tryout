using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomSingleAmbientSFX : MonoBehaviour
{
    public List<AudioClip> sounds;
    [Header("Settings")]
    public string soulIDForLoudness = "catSoul"; // Die ID deiner Katzen-Seele
    public float minWaitTime = 2f;
    public float maxWaitTime = 6f;

    [Header("Volume States")]
    public float normalVolume = 0.2f;
    public float loudVolume = 1.0f;

    private AudioSource mySource;
    private SoulManager soulManager;
    private float currentVolume;
    private bool hasCatSoul = false;

    void Start()
    {
        mySource = GetComponent<AudioSource>();
        mySource.spatialBlend = 0; // 2D Sound

        // Den SoulManager suchen
        GameObject gm = GameObject.Find("GameManager");
        if (gm != null)
        {
            soulManager = gm.GetComponent<SoulManager>();
        }

        // Initialer Check: Haben wir die Seele schon beim Start?
        CheckSoulStatus();

        if (sounds != null && sounds.Count > 0)
        {
            StartCoroutine(PlayLoop());
        }
    }

    void Update()
    {
        // Wir prüfen nur, solange wir die Seele noch nicht entdeckt haben
        if (!hasCatSoul)
        {
            CheckSoulStatus();
        }
    }

    private void CheckSoulStatus()
    {
        if (soulManager != null && soulManager.HasSoul(soulIDForLoudness))
        {
            hasCatSoul = true;
            currentVolume = loudVolume;
            Debug.Log("Ambient: Katzen-Seele erkannt! Volume auf laut.");
        }
        else
        {
            currentVolume = normalVolume;
        }
    }

    IEnumerator PlayLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            if (sounds.Count > 0)
            {
                AudioClip clip = sounds[Random.Range(0, sounds.Count)];

                // Nutzt die currentVolume, die in Update/Start gesetzt wird
                mySource.PlayOneShot(clip, currentVolume);
            }
        }
    }
}