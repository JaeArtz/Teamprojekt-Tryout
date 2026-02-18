using UnityEngine;

public class ParallaxSteadilyMovingImage : MonoBehaviour
{
    Material myMaterial;
    float distance;

    [Range(0f, 0.5f)]
    public float speed = 0.2f;


    /// <summary>
    /// Is called once before the first execution of Update after the MonoBehaviour is created.
    /// </summary>
    void Start()
    {
        myMaterial = GetComponent<Renderer>().material;
    }

    /// <summary>
    /// Is called once per frame.
    /// </summary>
    void Update()
    {
        distance += Time.deltaTime * speed;
        myMaterial.SetTextureOffset("_MainTex", Vector2.right * distance);
    }
}
