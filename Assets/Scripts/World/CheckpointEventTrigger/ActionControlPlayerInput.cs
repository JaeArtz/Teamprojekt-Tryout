using System.Collections;
using UnityEngine;

public class ActionControlPlayerInput : TriggerAction
{
    [Header("Input Control")]
    [SerializeField] private bool lockInput = false;
    [SerializeField] private bool unlockInput = false;

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        // Look in Parent (Player has many Components)
        PlayerController controller = ctx.PlayerTransform.GetComponentInParent<PlayerController>();

        if (controller != null)
        {
            if (lockInput)
            {
                controller.SetInputLocked(true);
            }
            else if (unlockInput)
            {                
                controller.SetInputLocked(false);

                // It was recommended to wait 1 Feame
                // to register the new state, before killing Action-Sequence
                yield return null;
            }
        }
        else
        {
            Debug.LogError("ActionControlPlayerInput: PlayerController auf " + ctx.PlayerTransform.name + " nicht gefunden!");
        }

        yield break;
    }
}


// puts "SetInputLocked" into a single script for better control
// Player Movement can be enabled and deactivated easier using separate srcipts
// Also: no meddling in other peoples scripts...!
