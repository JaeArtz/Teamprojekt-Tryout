using System.Collections;
using UnityEngine;
//this is supposed to play immediately on trigger of checkpoint ( queue/ block is optional)
public class ActionPlayParticles : TriggerAction
{
    [Header("Particle Systems")]
    public ParticleSystem[] systems;

    [Tooltip("How long to emit particles before stopping emission.")]
    public float emitDuration = 3f;

    [Tooltip("If true: StopEmitting (particles live out their lifetime). If false: StopEmittingAndClear.")]
    public bool letParticlesLiveOut = true;

    [Tooltip("If true, will restart systems from zero (Clear before Play).")]
    public bool restart = true;

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        if (systems == null || systems.Length == 0)
            yield break;

        // Start immediately
        foreach (var ps in systems) // for each particle system... and: (true) => includes ps of children
        {
            if (ps == null) continue;
            if (restart) ps.Clear(true);
            ps.Play(true);
        }

        // fire off particles immediately, does not block action chain
        if (emitDuration > 0f)
            StartCoroutine(StopLater());

        yield break;
    }

    private IEnumerator StopLater()
    {
        yield return new WaitForSeconds(emitDuration);

        var stopBehavior = letParticlesLiveOut
            ? ParticleSystemStopBehavior.StopEmitting
            : ParticleSystemStopBehavior.StopEmittingAndClear;

        foreach (var ps in systems)
        {
            if (ps == null) continue;
            ps.Stop(true, stopBehavior);
        }
    }
}