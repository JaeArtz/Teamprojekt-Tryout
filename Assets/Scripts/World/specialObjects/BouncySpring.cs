using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class BouncySpring : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float bounceForce = 22f;

    [Header("Name of Trigger")]
    [SerializeField] private string bounceTriggerName = "Bounce1";

    

    [Header("Audio Settings for Bounce Sound")]
    [SerializeField] private AudioClip[] bounceSounds;  // can be filled with Audioclips in Inspector

    [SerializeField, Range(0f, 1f)] private float volume = 0.7f;

    private Animator anim;
    private AudioSource audioSource;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Getting Box-Collider from Player- Child "Rotator"
        PlayerJump player = collision.GetComponentInParent<PlayerJump>();

        if (player != null)
        {
            // uses property float in PlayerMovement-Skript to set y-velocity
            player.Jump(yVel: bounceForce);

            // triggers animation for bounce
            if (anim != null)
            {
                anim.SetTrigger("Bounce1");
            }

            Debug.Log($"Bounce successfully executed. Goal-Objekt: {collision.gameObject.name}");

            PlayRandomSound();

            Debug.Log("Random Sound for Bounce should have been played.");
        }
        else
        {
            Debug.Log($"Something touched the shroom ({collision.gameObject.name}), but no movement or sound detected.");
        }

        
    }

    private void PlayRandomSound()
    {
        if (bounceSounds == null || bounceSounds.Length == 0) return;
        if (audioSource == null) return;

        int randomIndex = Random.Range(0, bounceSounds.Length);

        audioSource.PlayOneShot(bounceSounds[randomIndex], volume);
    }
}