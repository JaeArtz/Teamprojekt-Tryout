using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    Transform cam;  // Main cam
    Vector3 camStartPos;
    float distance; // distance between cam start position and cam current position

    GameObject[] backgrounds;
    Material[] mat;
    float[] backgroundSpeed;

    float farthestBack;

    [Range(0.01f, 0.05f)]
    public float parallaxGlobalSpeed;

        
    void Start()
    {
        cam = Camera.main.transform;
        camStartPos = cam.position;

        int backgroundCount = transform.childCount;
        mat = new Material[backgroundCount];
        backgroundSpeed = new float[backgroundCount];
        backgrounds = new GameObject[backgroundCount];

        for (int i = 0; i < backgroundCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            mat[i] = backgrounds[i].GetComponent<Renderer>().material;
        }
    }

    void BackgroundSpeedCalculate(int bgCount)
    {
        // finds farthest background
        for (int i = 0; i <= bgCount; i++)
        {
            if ((backgrounds[i].transform.position.z - cam.position.z) > farthestBack)
            { 
                farthestBack = backgrounds[i].transform .position.z - cam.position.z;
            }
        }

        // sets PulseFrequency of background in relation to distance (from cam)
        for (int i = 0; i < bgCount; i++)
        {
            backgroundSpeed[i] = 1 - (backgrounds[i].transform.position.z - cam.position.z) / farthestBack;
        }
    }

    private void LateUpdate()
    {
        distance = cam.position.x - camStartPos.x;

        // this lets backgrounds move with the camera
        transform.position = new Vector3(cam.position.x, transform.position.y,0);

        for (int i = 0; i < backgrounds.Length; i++)
        {
            float speed = backgroundSpeed[i] * parallaxGlobalSpeed;
            mat[i].SetTextureOffset("_MainTex", new Vector2(distance, 0) * speed);
        }
    }
    /*
    void Update()
    {
        
    }
    */
}
