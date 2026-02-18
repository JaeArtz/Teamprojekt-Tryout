using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxRepeatTwo : MonoBehaviour
{
    public Transform cam;

    /// <summary>
    /// 1 = camspeed, 0 = "standing still", 0.5 = "passing by slowly"
    /// </summary>
    [Range(0f, 2f)]
    public float parallaxMultiplier = 1f;

    private Vector3 lastCamPos;
    private float length;   

    private void Start()
    {
        if (cam == null)
            cam = Camera.main.transform;

        lastCamPos = cam.position;

        var sr = GetComponent<SpriteRenderer>();
        length = sr.bounds.size.x;  // // sprite width inside world/ ingame
    }

    private void LateUpdate()
    {
        Vector3 delta = cam.position - lastCamPos;

        // creating Parallax Movement in relation to Vector3
        transform.position += new Vector3(delta.x * parallaxMultiplier, 0f, 0f);

        lastCamPos = cam.position;

        // creating infinite loop
        float diff = cam.position.x - transform.position.x;

        if (Mathf.Abs(diff) >= length)
        {
            float offset = (diff > 0) ? length : -length;
            transform.position += new Vector3(offset, 0f, 0f);
        }
    }
}
