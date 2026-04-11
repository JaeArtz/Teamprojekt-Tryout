using System.Collections;
using UnityEngine;

public class ActionSelfDestruct : TriggerAction
{
    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        // SelfDestruct
        Destroy(gameObject);
        yield break;
    }
}