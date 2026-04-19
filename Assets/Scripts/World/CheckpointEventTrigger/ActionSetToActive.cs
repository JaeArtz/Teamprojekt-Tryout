using System.Collections;
using UnityEngine;

public class ActionSetToActive : TriggerAction
{
    public GameObject targetObject; 
    [Tooltip("If checked, Object will be avctivated on call of script. If unchecked, object will be DE-activated on call of script.")]
    public bool activateObject = true;
    [Tooltip("How many seconds should be waited until start of next Action.")]
    public float delayAfterActivation = 0f;

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        if (targetObject != null)
        {
            targetObject.SetActive(activateObject);
        }

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
}
