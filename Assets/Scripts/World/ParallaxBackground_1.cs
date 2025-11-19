using UnityEngine;

public class ParallaxBackground_1 : MonoBehaviour
{
    public Transform cam;                // Referenz zur Kamera
    [Range(0f, 1f)]
    public float parallaxMultiplier = 0.1f; // 0 = fest, 1 = bewegt sich wie Vordergrund

    private Vector3 lastCamPos;

    void Start()
    {
        if (cam == null)
            cam = Camera.main.transform;

        lastCamPos = cam.position;
    }

    /*   BEWEGUNG IN BEIDE RICHTUNGEN, bewegt sich aber dann mit player
    void LateUpdate()
    {
        Vector3 delta = cam.position - lastCamPos;

        // Nur in X (und optional Y) bewegen:
        transform.position += new Vector3(delta.x * parallaxMultiplier,
                                          delta.y * parallaxMultiplier,
                                          0f);

        lastCamPos = cam.position;
    }
    */

    void LateUpdate()
    {
        Vector3 delta = cam.position - lastCamPos;

        // Nur horizontale Parallax-Bewegung
        transform.position += new Vector3(delta.x * parallaxMultiplier, 0f, 0f);

        lastCamPos = cam.position;
    }

}
