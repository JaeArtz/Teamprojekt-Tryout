using UnityEngine;
using System.Collections.Generic;

public class BreakableWall : MonoBehaviour
{
    private AudioSource audioSource;
    private Animator animator;

    [Header("Sound Settings")]
    public List<AudioClip> breakSounds;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    public void Break()
    {
        // 3. SOUND SOFORT ABFEUERN
        PlayRandomSound();

        // 1. RADIKAL: Alle Kinder (deine Mauer-Grafik) sofort deaktivieren
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        // 2. COLLIDER SOFORT AUS
        // Damit der Player nicht an einer unsichtbaren Wand hängen bleibt
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

       

        // 4. SPRENG-ANIMATION STARTEN
        // Diese Animation muss im Animator auf dem Parent liegen!
        if (animator != null)
        {
            animator.SetTrigger("Break");
        }

        // 5. OBJEKT LÖSCHEN
        // Wir warten 3 Sekunden, damit Sound und Animation fertig spielen können
        Destroy(gameObject, 3.0f);
    }

    private void PlayRandomSound()
    {
        if (audioSource != null && breakSounds.Count > 0)
        {
            audioSource.PlayOneShot(breakSounds[Random.Range(0, breakSounds.Count)]);
        }
    }
}