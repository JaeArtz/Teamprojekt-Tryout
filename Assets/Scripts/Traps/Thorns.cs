using UnityEngine;

public class Thorns : MonoBehaviour
{
    // Thorns don't have to damage every single time- can only make a sound
    // simulates "real" bushes, with Thorns: can damage you- or not if you are lucky


    private Animator anim;
    private AudioSource audioSource;

    [Header("Visual Settings")]
    [Tooltip("Hit is Trigger for Controler to start Animation of ThornWiggling")]
    [SerializeField] private string triggerName = "Hit"; // ANIMATION Trigger

    [Header("Audio Settings")]
    [Tooltip("Drag Audio Files for Thorns in here, will be played randomly at contact")]
    [SerializeField] private AudioClip[] thornSounds;
    [Range(0f, 1f)][SerializeField] private float volume = 0.7f;

    [Header("Cooldown")]
    [Tooltip("Cooldown until next Wiggling is allowed")]
    [SerializeField] private float visualCooldown = 0.2f;
    private float nextTriggerTime;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        // finds Audio Source for random RustleSound
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || (collision.transform.parent != null && collision.transform.parent.CompareTag("Player")))
        {
            // Check if enough time has passed to prevent double-firing
            if (Time.time >= nextTriggerTime)
            {
                PlayRandomSound();
                TriggerThornAnimation();
                nextTriggerTime = Time.time + visualCooldown;
            }
        }
    }

    private void TriggerThornAnimation()
    {
        if (anim != null)
        {
            anim.SetTrigger(triggerName);
        }
    }

    private void PlayRandomSound()
    {
        if (thornSounds != null && thornSounds.Length > 0)
        {
            // random index of List, picks Sound randomly
            int randomIndex = Random.Range(0, thornSounds.Length);
            AudioClip clipToPlay = thornSounds[randomIndex];

            if (clipToPlay != null)
            {
                // plays Sounds without Queue, can overlap
                audioSource.PlayOneShot(clipToPlay, volume);
            }
        }
    }
}