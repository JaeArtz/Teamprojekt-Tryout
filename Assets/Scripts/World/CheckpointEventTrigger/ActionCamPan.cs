using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class ActionCamPan : TriggerAction
{
    [Header("Target")]
    [Tooltip("Cam moves here, and then back to Player (drag in the PointObject)")]
    public Transform targetPoint;
    public bool returnToPlayer = false;

    [Header("Cam Settings")]
    [Tooltip("ActionCamPan Duration")]
    public float duration = 2.0f;
    [Tooltip("Smoothness/ Curve of Movement")]
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        // 1. Cam must be assigned in Inspector
        if (ctx.VirtualCamera == null) yield break;

        Transform camFollowTarget = ctx.VirtualCamera.Follow;
        if (camFollowTarget == null) yield break;

        // 2. Temporarily takes CamTracker (=Cam) from Player
        // Player stays behind
        if (!returnToPlayer && camFollowTarget.parent != null)
        {
            // Later Player will be Paretn again
            camFollowTarget.SetParent(null);
        }

        Vector3 startPos = camFollowTarget.position;
        Vector3 endPos = returnToPlayer ? ctx.PlayerTransform.position : targetPoint.position;

        // 3. The Moving of Cam
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = curve.Evaluate(elapsed / duration);

            // We know where the Player is, and find his positionif necessary             
            Vector3 currentTarget = returnToPlayer ? ctx.PlayerTransform.position : targetPoint.position;

            camFollowTarget.position = Vector3.Lerp(startPos, currentTarget, t);
            yield return null;
        }

        camFollowTarget.position = endPos;

        // 4. Re-attach Cam to Player
        if (returnToPlayer)
        {
            camFollowTarget.SetParent(ctx.PlayerTransform);
            camFollowTarget.localPosition = Vector3.zero; // zero = "Centre" on Player
        }
    }
}