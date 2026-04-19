using System.Collections;
using UnityEngine;

public class ActionWait : TriggerAction
{

    [Tooltip("How many seconds to wait before the next action starts.")]
    public float waitTime = 1f;

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        // We force a block for the set amount of seconds
        if (waitTime > 0)
        {
            yield return new WaitForSeconds(waitTime);
        }
        yield break;
    }
}