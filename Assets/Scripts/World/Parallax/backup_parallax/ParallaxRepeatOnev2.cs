using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ParallaxRepeatOnev2 : MonoBehaviour
{
    // uses Main cam of scene
    public Transform cam;                 
    [Range(0f, 1f)]

    // 0 = "standing still" , 1 = "moves with foreground"
    public float parallaxMultiplier = 0.2f; 

    private Material mat;
    private Vector3 lastCamPos;
    private Vector2 texOffset;

    void Start()
    {
        if (cam == null)
            cam = Camera.main.transform;

        // flat copy - doesn'tchange global matieral, only uses it
        mat = GetComponent<Renderer>().material;

        lastCamPos = cam.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cam.position - lastCamPos;

        // horizontal "sliding"
        // 0.1f = scaling factor, scalingadjustable
        texOffset.x += delta.x * parallaxMultiplier * 0.1f;
       

        mat.mainTextureOffset = texOffset;

        lastCamPos = cam.position;
    }
}


