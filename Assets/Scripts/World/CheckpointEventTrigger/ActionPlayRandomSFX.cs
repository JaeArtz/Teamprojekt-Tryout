using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPlayRandomSFX : TriggerAction
{
    public List<AudioClip> sounds;
    [Range(0, 1)] public float volume = 1f;

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        if (sounds != null && sounds.Count > 0)
        {
            AudioClip clip = sounds[Random.Range(0, sounds.Count)];
            // Wir nehmen die Position der Wand (transform.position)
            AudioSource.PlayClipAtPoint(clip, transform.position, volume);
        }
        yield break;
    }
}