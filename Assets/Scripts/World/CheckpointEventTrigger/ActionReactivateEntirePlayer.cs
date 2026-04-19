using System.Collections;
using UnityEngine;

public class ActionReactivateEntirePlayer : TriggerAction
{
    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        if (ctx.PlayerObject == null) yield break;

        // all components are deactivated, SpriteRenderer, Scripts... So: get ALL
        Component[] allComponents = ctx.PlayerObject.GetComponents<Component>();

        foreach (var comp in allComponents)
        {
            if (comp == null) continue;

            // Everything that can be enabled, will be enabled
            var prop = comp.GetType().GetProperty("enabled");
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(comp, true, null);
            }
        }

        // Rigidbody2D is a little different, get it this way
        var rb = ctx.PlayerObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
            rb.WakeUp();
        }

        yield break;
    }
}