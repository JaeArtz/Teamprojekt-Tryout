using UnityEngine;

public class DesertWindFollow : MonoBehaviour
{
    [SerializeField] private Transform targetCamera;

    private void LateUpdate()
    {
        if (targetCamera != null)
        {
            // Das Kind bewegt sich UNABHÄNGIG vom Parent zur Kamera
            transform.position = new Vector3(targetCamera.position.x, targetCamera.position.y, 0);
        }
    }
}