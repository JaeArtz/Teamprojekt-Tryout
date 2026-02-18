using System.Collections;
using UnityEngine;

public class ActionPlaySFX : MonoBehaviour
{

    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;

    public IEnumerator Execute(TriggerInfoBundle ctx)
    {
        if (clip == null) yield break;

        var source = GetComponent<AudioSource>();
        if (source == null) source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;

        source.PlayOneShot(clip, volume);
        yield break; // non-blocking
    }
}
