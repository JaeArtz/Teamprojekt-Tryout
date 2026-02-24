using System.Collections;
using UnityEngine;

public class ActionBlowSand : TriggerAction
{
    [Header("Sand Animator")]
    public Animator sandAnimator;

    [Header("Trigger Name")]
    public string triggerName = "Blow";

    [Header("Wind Sound")]
    public AudioSource windSource;

    public AudioClip windClip;

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        if (sandAnimator != null)
        {
            sandAnimator.SetTrigger(triggerName);
        }

        if (windSource != null && windClip != null)
        {
            windSource.PlayOneShot(windClip);
        }

        yield break; // non-blocking
    }
}