using System.Collections;
using UnityEngine;

public class ActionSpawnPrefab : TriggerAction
{
    public GameObject prefab;
    public Transform spawnPoint; // optional
    public bool parentToTrigger = false;
    public bool destroyAfterSeconds = true;
    public float lifetime = 5f;

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        if (prefab == null) yield break;

        var t = spawnPoint != null ? spawnPoint : transform;
        var go = Instantiate(prefab, t.position, t.rotation);

        if (parentToTrigger) go.transform.SetParent(ctx.TriggerObject.transform);

        if (destroyAfterSeconds && lifetime > 0f)
            Destroy(go, lifetime);

        yield break;
    }
}
