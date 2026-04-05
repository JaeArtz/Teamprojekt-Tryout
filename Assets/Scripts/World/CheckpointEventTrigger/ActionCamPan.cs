using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class ActionCamPan : TriggerAction
{
    [Header("Target")]
    public Transform targetPoint;
    public bool returnToPlayer = false;

    [Header("Settings")]
    public float duration = 2.0f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        // 1. Sicherheitcheck: Haben wir eine Kamera im Inspektor zugewiesen?
        if (ctx.VirtualCamera == null) yield break;

        Transform camFollowTarget = ctx.VirtualCamera.Follow;
        if (camFollowTarget == null) yield break;

        // 2. Den Punkt von der Player-Hierarchie lösen (nur beim Hinweg)
        // Das verhindert, dass der Player die Bewegung "blockiert"
        if (!returnToPlayer && camFollowTarget.parent != null)
        {
            // Wir merken uns den Player, um später wieder zurückzufinden
            camFollowTarget.SetParent(null);
        }

        Vector3 startPos = camFollowTarget.position;
        Vector3 endPos = returnToPlayer ? ctx.PlayerTransform.position : targetPoint.position;

        // 3. Die eigentliche Bewegung
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = curve.Evaluate(elapsed / duration);

            // Falls der Player sich während des Rückwegs minimal bewegt, 
            // aktualisieren wir die Zielposition
            Vector3 currentTarget = returnToPlayer ? ctx.PlayerTransform.position : targetPoint.position;

            camFollowTarget.position = Vector3.Lerp(startPos, currentTarget, t);
            yield return null;
        }

        camFollowTarget.position = endPos;

        // 4. Den Punkt wieder an den Player hängen (nur beim Rückweg)
        if (returnToPlayer)
        {
            camFollowTarget.SetParent(ctx.PlayerTransform);
            camFollowTarget.localPosition = Vector3.zero; // Wieder genau auf den Player zentrieren
        }
    }
}