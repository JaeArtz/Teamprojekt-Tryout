using UnityEngine;

public class ParallaxRepeatOneV3 : MonoBehaviour
{
    Material mat;
    Vector3 lastCamPosition;
    Transform cam;

    // controls how "strongly" the layer moves in relation to Main cam
    // measures distance (how close or far away => factor Z)
    [Range(0f, 1f)]
    public float parallaxIndividualSpeedFactor = 0.5f;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        cam = Camera.main.transform;
        lastCamPosition = cam.position;
    }

    void Update()
    {
        Vector3 deltaMovement = cam.position - lastCamPosition;

        // changing Textur-Offset for horizontal "slide" in relation to Main cam movement
        mat.mainTextureOffset += new Vector2(deltaMovement.x * parallaxIndividualSpeedFactor, 0f);

        lastCamPosition = cam.position;
    }
}
