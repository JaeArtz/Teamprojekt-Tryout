using System.Collections;
using UnityEngine;

public abstract class TriggerAction : MonoBehaviour
{
    [Tooltip("If true: waits until action is finished before starting next action.")]
    public bool blockUntilFinished = false;

    public abstract IEnumerator Execute(TriggerInfoBundle ctx);
}
