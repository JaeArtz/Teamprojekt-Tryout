using System.Collections;
using UnityEngine;

public class ActionSwapParallaxBackground : TriggerAction
{
    [Header("Target Parallax Object")]
    public ParallaxLayer_BuiltIn_2 target;

    [Header("New Background")]
    public Texture newTexture;

    public bool resetOffset = true;

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        if (newTexture == null) yield break;

        if (target == null)
            target = FindFirstObjectByType<ParallaxLayer_BuiltIn_2>();

        if (target == null)
        {
            Debug.LogWarning("No ParallaxLayer_BuiltIn_2 found in scene.");
            yield break;
        }

        target.SetTexture(newTexture, resetOffset);
        yield break;
    }
}