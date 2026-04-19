using System.Collections;
using UnityEngine;

public class ActionSetToActive : TriggerAction
{   

    public GameObject targetObject;
    [Tooltip("If checked, Object will be activated on call of script. If unchecked, object will be DE-activated on call of script.")]
    public bool activateObject = true;
    [Tooltip("How many seconds should be waited until start of next Action.")]
    public float delayAfterActivation = 0f;

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        if (targetObject == null) yield break;

        if (activateObject)
        {
            // nomalMode: targetObject will be activated right away
            targetObject.SetActive(true);

            // here, the waiting part is integrated with "WaitForSeconds"
            if (delayAfterActivation > 0)
            {
                yield return new WaitForSeconds(delayAfterActivation);
            }
            else
            {
                yield break;
            }
        }
        else
        {
            // --- Killswitch Mode ---
            // if we selfdestruct, we wait until kill (Timer)
            if (delayAfterActivation > 0)
            {
                yield return new WaitForSeconds(delayAfterActivation);
            }

            // after delayAfterActivation-Time, KillSwitch ist pulled
            targetObject.SetActive(false);

            yield break;
        }
    }
}