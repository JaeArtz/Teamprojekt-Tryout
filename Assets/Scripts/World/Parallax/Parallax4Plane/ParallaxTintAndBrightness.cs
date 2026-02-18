using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ParallaxTintAndBrightness : MonoBehaviour
{
    [Range(0f, 2f)]
    public float brightness = 1f;   // 1 = normal, standard brightness

    public Color tint = Color.white; // white = no change

    Renderer rend;
    Material mat;

    void Start()
    {
        rend = GetComponent<Renderer>();
        mat = rend.material;     // flat copy
    }

    void Update()
    {
        // titn = base color
        mat.color = tint * brightness;
    }
}
