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
        // 3. Sound
        PlayRandomSound();

        // 1. deactivate children (= only Wall-Sprite, turns Wall invisuble)
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        // 2. Collider off
        // (Turns off "invisible Wall"-Box)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

       

        // 4. Animation (CrushCrumble) on Parent       
        if (animator != null)
        {
            animator.SetTrigger("Break");
        }

        // 5. Delet Object completely after x seconds        
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