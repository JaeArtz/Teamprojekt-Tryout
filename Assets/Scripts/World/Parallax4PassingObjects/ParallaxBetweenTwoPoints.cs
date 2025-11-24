using UnityEngine;

public class ParallaxBetweenTwoPoints : MonoBehaviour
{
    [Header("Parallax")]
    public Transform cam;
    [Range(0f, 1f)]
    public float parallaxMultiplier = 0.1f;

    [Header("Set Limits (position in Scene)")]
    public Transform leftPoint;   // maximum of pivot point left
    public Transform rightPoint;  // maximum of pivot point right

    private Vector3 lastCamPosition;

    private float leftX;
    private float rightX;

    void Start()
    {
        if (cam == null)
            cam = Camera.main.transform;

        lastCamPosition = cam.position;

        // saves coordinates once at start
        if (leftPoint != null) leftX = leftPoint.position.x;
        if (rightPoint != null) rightX = rightPoint.position.x;
    }

    void LateUpdate()
    {
        // Parallax Multiplier like other Parallax Scripts
        Vector3 delta = cam.position - lastCamPosition;
        transform.position += new Vector3(delta.x * parallaxMultiplier, 0f, 0f);
        lastCamPosition = cam.position;

        // checks x really hard for right and left limit
        Vector3 p = transform.position;

        if (leftPoint != null && p.x < leftX)
            p.x = leftX;

        if (rightPoint != null && p.x > rightX)
            p.x = rightX;

        transform.position = p;
    }
}
