using UnityEngine;
using System.Collections.Generic; // Ermöglicht die Liste

public class BossEndTrigger : MonoBehaviour
{
    [Header("Referenzen")]
    public GameObject golemDebris;
    public GameObject bossFightContainer;

    [Header("Finale Platformen")]
    // Hier kannst du im Inspector so viele Plattformen reinziehen wie du willst
    public List<GameObject> finalPlatforms = new List<GameObject>();

    [Header("Audio")]
    public AudioSource collapseSound;

    public void StopTheBoss()
    {
        // 1. Krach abspielen
        if (collapseSound != null) collapseSound.Play();

        // 2. Boss aus
        if (bossFightContainer != null) bossFightContainer.SetActive(false);

        // 3. Trümmer an
        if (golemDebris != null) golemDebris.SetActive(true);

        // --- NEU: Alle finalen Plattformen aktivieren ---
        foreach (GameObject platform in finalPlatforms)
        {
            if (platform != null)
            {
                platform.SetActive(true);
                // Optional: Hier könnte man später noch einen kleinen 
                // Partikeleffekt pro Plattform spawnen lassen
            }
        }

        Debug.Log("Boss besiegt, Trümmer da und Pfad nach oben ist frei!");
    }
}