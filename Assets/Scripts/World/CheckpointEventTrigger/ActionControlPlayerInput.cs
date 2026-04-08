using System.Collections;
using UnityEngine;

public class ActionControlPlayerInput : TriggerAction
{
    [Header("Input Control")]
    [SerializeField] private bool lockInput = false;
    [SerializeField] private bool unlockInput = false;

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        // Wir suchen den Controller sicher im Parent (da der Player oft aus mehreren Objekten besteht)
        PlayerController controller = ctx.PlayerTransform.GetComponentInParent<PlayerController>();

        if (controller != null)
        {
            if (lockInput)
            {
                controller.SetInputLocked(true);
            }
            else if (unlockInput)
            {
                // Wir setzen den Lock auf false
                controller.SetInputLocked(false);

                // WICHTIG: Wir warten einen Frame, damit der Controller 
                // den neuen Status intern registrieren kann, bevor die Action-Sequenz stirbt.
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
