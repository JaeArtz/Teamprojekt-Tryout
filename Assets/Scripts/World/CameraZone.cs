using Unity.Cinemachine;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
    public CinemachineCamera zoneCamera;
    public int activePriority = 20;
    public int inactivePriority = 0;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            zoneCamera.Priority = activePriority;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            zoneCamera.Priority = inactivePriority;
    }
}