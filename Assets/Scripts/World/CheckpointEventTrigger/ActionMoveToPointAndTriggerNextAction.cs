using System.Collections;
using UnityEngine;

public class ActionMoveToPointAndTriggerNextAction : TriggerAction
{
    [Header("Movement")]
    [Tooltip("Drag in object that is supposed to m,ove towards targetPopint.")]
    public Transform objectToMove;
    [Tooltip("Drag in emptyObject (or other) that serves as targetPoint for movement")]
    public Transform targetPoint;
    [Tooltip("Movement Speed of moving object")]
    public float speed = 5f;
    [Tooltip("Offset for x coordinate of moving objext: object takes on x-value of targetPoint + xOffset")]
    public float xOffset = 0f;

    [Header("Post-Arrival")]
    [Tooltip("Should the object self-destruct upon arrival?")]
    public bool selfDestructOnArrival = true;

    // GEMINI: Variable, die dem System signalisiert, dass wir noch arbeiten
    public bool isFinished = false;

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        isFinished = false; // GEMINI: Wir fangen gerade erst an

        if (objectToMove == null || targetPoint == null)
        {
            isFinished = true;
            yield break;
        }

        float targetX = targetPoint.position.x + xOffset;
        objectToMove.position = new Vector3(targetX, objectToMove.position.y, objectToMove.position.z);

        // object moves until <= y of targetPoint
        while (objectToMove != null && objectToMove.position.y > targetPoint.position.y)
        {
            objectToMove.position = Vector3.MoveTowards(
                objectToMove.position,
                new Vector3(targetX, targetPoint.position.y, objectToMove.position.z),
                speed * Time.deltaTime
            );
            yield return null;
        }

        // positions object at targetPoint, taking into account the x-value + offset
        if (objectToMove != null)
        {
            objectToMove.position = new Vector3(targetX, targetPoint.position.y, targetPoint.position.z);
        }

        if (selfDestructOnArrival && objectToMove != null)
        {
            Destroy(objectToMove.gameObject);
        }

        // GEMINI: Erst HIER setzen wir isFinished auf true
        isFinished = true;

        // Golden Tear hits the Ground, next Animation should start now in Sequence
    }
}