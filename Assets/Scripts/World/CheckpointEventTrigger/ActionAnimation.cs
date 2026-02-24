using System.Collections;
using UnityEngine;

public class ActionAnimation: TriggerAction
{
    public Animator animator;
    public string triggerName = "Play";
    public bool waitForStateToFinish = false;

    [Tooltip("Optional: name of state you are waiting for. empty => 1 second fallback.")]
    public string stateName = "";

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        if (animator == null) yield break;

        animator.SetTrigger(triggerName);

        if (!waitForStateToFinish || !blockUntilFinished)
            yield break;

       
        if (!string.IsNullOrEmpty(stateName))
        {
            // waits until state is active and until normalizedTime >= 1
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
                yield return null;

            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                yield return null;
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }
    }
}
