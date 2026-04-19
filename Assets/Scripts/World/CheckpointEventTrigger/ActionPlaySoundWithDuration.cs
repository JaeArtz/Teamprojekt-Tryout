using System.Collections;
using UnityEngine;

public class ActionPlaySoundWithDuration : TriggerAction
{
    public AudioSource audioSource;
    [Tooltip("How many seconds should the sound be played? ( 0 = play fully until end of file)")]
    public float playDuration = 2f;
    [Tooltip("Should be waited until end of duration?")]
    public bool waitForDuration = true;

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        if (audioSource == null) yield break;

        audioSource.Play();

        // if duration = 0 => playfull length uf Audio
        if (playDuration <= 0) yield break;

        if (waitForDuration)
        {
            // pause for length of set "duration"
            yield return new WaitForSeconds(playDuration);
            audioSource.Stop();
        }
        else
        {
            // The next Action can start, but the Audio keeps playing
            ctx.TriggerObject.GetComponent<MonoBehaviour>().StartCoroutine(StopAfterDelay(playDuration));
        }
    }

    private IEnumerator StopAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (audioSource != null) audioSource.Stop();
    }
}