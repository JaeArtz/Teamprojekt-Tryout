using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class ActionTimelineCutscene : TriggerAction
{
    [Header("Timeline")]
    public PlayableDirector director;

    [Header("Player Lock")]
    [Tooltip("Used when \"pausing\" game for cutscene")]
    public bool lockPlayerMovement = true;
    [Tooltip("Wenn true: enabled = false. When false: tries to call SendMessage 'SetInputEnabled(bool)'.")]
    public bool hardDisableMovementComponent = true;

    [Header("Camera Zoom (Cinemachine 3)")]
    public bool doZoom = true;
    public float zoomOutOrthoSize = 10f;
    public float zoomDuration = 0.7f;

    private float _originalOrthoSize;

    public override IEnumerator Execute(TriggerInfoBundle ctx)
    {
        // For Cutscene probably always good:
        // In Inspector: set blockUntilFinished = true

        // Player lock
        if (lockPlayerMovement && ctx.PlayerMovement != null)
        {
            if (hardDisableMovementComponent)
                ctx.PlayerMovement.enabled = false;
            else
                ctx.PlayerMovement.SendMessage("SetInputEnabled", false, SendMessageOptions.DontRequireReceiver);
        }

        // Zoom out
        if (doZoom && ctx.VirtualCamera != null)
        {
            _originalOrthoSize = GetOrthoSize(ctx.VirtualCamera);
            yield return LerpOrtho(ctx.VirtualCamera, _originalOrthoSize, zoomOutOrthoSize, zoomDuration);
        }

        // Timeline play + wait
        if (director != null)
        {
            director.time = 0;
            director.Play();

            while (director.state == PlayState.Playing)
                yield return null;
        }

        // Zoom back
        if (doZoom && ctx.VirtualCamera != null)
        {
            yield return LerpOrtho(ctx.VirtualCamera, GetOrthoSize(ctx.VirtualCamera), _originalOrthoSize, zoomDuration);
        }

        // Player unlock
        if (lockPlayerMovement && ctx.PlayerMovement != null)
        {
            if (hardDisableMovementComponent)
                ctx.PlayerMovement.enabled = true;
            else
                ctx.PlayerMovement.SendMessage("SetInputEnabled", true, SendMessageOptions.DontRequireReceiver);
        }
    }

    private IEnumerator LerpOrtho(CinemachineCamera cam, float from, float to, float dur)
    {
        if (dur <= 0f)
        {
            SetOrthoSize(cam, to);
            yield break;
        }

        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / dur);
            SetOrthoSize(cam, Mathf.Lerp(from, to, a));
            yield return null;
        }

        SetOrthoSize(cam, to);
    }

    // --- Cinemachine 3 Lens helpers ---
    private float GetOrthoSize(CinemachineCamera cam)
    {
        // Cinemachine 3: lens is usually in cam.Lens
        // (test this first, I am not sure yet this is correct)
        return cam.Lens.OrthographicSize;
    }

    private void SetOrthoSize(CinemachineCamera cam, float size)
    {
        var lens = cam.Lens;
        lens.OrthographicSize = size;
        cam.Lens = lens; // because lens is probably a struct
    }
}
