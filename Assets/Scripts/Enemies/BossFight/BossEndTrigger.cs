using UnityEngine;
using System.Collections.Generic; // Enables creating the List

public class BossEndTrigger : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Object holding the DebrisSprites, is deactivated per default")]
    public GameObject golemDebris;
    [Tooltip("Object holding the GolemLimbs and FightAnimationStuff, is deactivated per default")]
    public GameObject bossFightContainer;

    [Header("Final Platforms")]
    [Tooltip("Drag the Special Ground-Tilemap in here: These glowing platforms and everything with it appear after GolemBoss is defeated, deactivated per default")]
    public List<GameObject> finalPlatforms = new List<GameObject>();

    [Header("Audio")]
    public AudioSource collapseSound;

    public void StopTheBoss()
    {
        // 1. Play Sound
        if (collapseSound != null) collapseSound.Play();

        // 2. BossFight Animation OFF
        if (bossFightContainer != null) bossFightContainer.SetActive(false);

        // 3. GolemDebris ON
        if (golemDebris != null) golemDebris.SetActive(true);

        // 4. Activate glowing platforms, and phoenix soul for "FInale"
        foreach (GameObject platform in finalPlatforms)
        {
            if (platform != null)
            {
                platform.SetActive(true);                
            }
        }

        Debug.Log("Boss defeated, Debris and GlowingPlatforms appeared!");
    }
}