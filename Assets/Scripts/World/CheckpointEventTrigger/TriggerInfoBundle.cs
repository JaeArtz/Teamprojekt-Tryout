using Unity.Cinemachine;
using UnityEngine;

// bundle of Info to hand over to an Action/Event, that it might need for executing
public class TriggerInfoBundle
{
    public GameObject TriggerObject { get; }
    public GameObject PlayerObject { get; }
    public Transform PlayerTransform { get; }
    public MonoBehaviour PlayerMovement { get; }
    public CinemachineCamera VirtualCamera { get; }

    public TriggerInfoBundle(
        GameObject triggerObject,
        GameObject playerObject,
        MonoBehaviour playerMovement,
        CinemachineCamera virtualCamera)
    {
        TriggerObject = triggerObject;
        PlayerObject = playerObject;
        PlayerTransform = playerObject != null ? playerObject.transform : null;
        PlayerMovement = playerMovement;
        VirtualCamera = virtualCamera;
    }
}
