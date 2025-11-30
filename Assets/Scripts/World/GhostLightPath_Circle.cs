using UnityEngine;

public class GhostLightPath_Circle : MonoBehaviour
{
  

    public float radiusWidth = 1f;   //width of circle/ ellipse
    public float radiusHeight = 0.5f; // radius (height) of circle/ ellipse
    public float MovingSpeed = 1f;   // floating/moving MovingSpeed
    public float offset = 0f;   // offset in case there are several lights on same path

    private Vector3 center;

    void Start()
    {
        // center= center point of circle or ellipse form (starting position in scene)
        center = transform.position;
    }

    void Update()
    {
        float t = Time.time * MovingSpeed + offset;
        float x = Mathf.Cos(t) * radiusWidth;
        float y = Mathf.Sin(t) * radiusHeight;

        transform.position = center + new Vector3(x, y, 0f);
    }
}


