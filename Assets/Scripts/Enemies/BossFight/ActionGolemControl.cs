using System.Collections;
using UnityEngine;

public class ActionGolemControl : TriggerAction
{
    public GolemBoss golem;
    public bool wakeUp = true;

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        if (golem == null) yield break;

        if (wakeUp)
        {
            golem.WakeUp();
        }
        else
        {
            golem.currentState = GolemBoss.GolemState.Defeated;
        }

        yield break;
    }
}