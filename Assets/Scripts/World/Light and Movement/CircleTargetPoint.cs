using UnityEngine;

public class CircleTargetPoint : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Drag Player in here")]
    public Transform targetPoint;

    [Header("Circle Movement")]
    public float radiusWidth = 1f;
    public float radiusHeight = 0.5f;
    public float movingSpeed = 1f;
    public float offset = 0f;

    void Update()
    {
       
        if (targetPoint == null) return;

       
        float t = Time.time * movingSpeed + offset;
        float x = Mathf.Cos(t) * radiusWidth;
        float y = Mathf.Sin(t) * radiusHeight;

        
        transform.position = targetPoint.position + new Vector3(x, y, 0f);
    }
}