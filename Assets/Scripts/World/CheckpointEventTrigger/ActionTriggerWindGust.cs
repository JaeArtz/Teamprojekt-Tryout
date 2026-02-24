using System.Collections;
using UnityEngine;

public class ActionTriggerWindGust : TriggerAction
{
    [Header("Wind Controller")]
    public WindController windController;

    [Header("Gust Settings")]
    public float duration = 2f;
    public float influence = 1.2f;
    public float speed = 9f;

    [Header("One-Shot Sound")]
    public AudioSource audioSource;
    public AudioClip windClip;
    [Range(0f, 2f)] public float volume = 1f;

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        if (windController == null)
            windController = WindController.Instance;

        if (windController != null)
        {
            // Random-Sound off
            windController.TriggerGust(duration, influence, speed, false);
        }

        // Listen closely, for I shall only play this once
        if (audioSource != null && windClip != null)
        {
            audioSource.PlayOneShot(windClip, volume);
        }

        yield break;
    }
}