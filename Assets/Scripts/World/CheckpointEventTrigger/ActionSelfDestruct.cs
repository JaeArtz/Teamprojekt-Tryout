using System.Collections;
using UnityEngine;

public class ActionSelfDestruct : TriggerAction
{
    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        // Löscht das Objekt, an dem diese Action (bzw. der Trigger) hängt
        Destroy(gameObject);
        yield break;
    }
}