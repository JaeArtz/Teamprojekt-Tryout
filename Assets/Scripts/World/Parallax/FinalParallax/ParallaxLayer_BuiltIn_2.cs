using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ParallaxLayer_BuiltIn_2 : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform cam;

    [Header("Plane follows camera (X only)")]
    [Tooltip("Plane will stay centered under the camera on X. Y/Z remain as placed.")]
    [SerializeField] private float planeXOffset = 0f;

    [Header("Parallax UV scroll (only when camera moves)")]
    [Tooltip("How much the texture UV scrolls per 1 world unit camera movement on X. Start tiny (0.0001 - 0.01).")]
    [SerializeField] private float uvScrollPerWorldUnitX = 0.002f;

    [Tooltip("Invert direction if it feels wrong.")]
    [SerializeField] private bool invert = false;

    [Header("Material property (Built-In)")]
    [SerializeField] private string textureProperty = "_MainTex";

    [Header("Stability")]
    [Tooltip("Clamps camera delta per frame (world units). Helps with camera snaps/teleports.")]
    [SerializeField] private float maxCamDeltaPerFrame = 5f;

    private Material mat;
    private Vector3 startPos;
    private float lastCamX;
    private float uvX;

    void Awake()
    {
        if (cam == null && Camera.main != null) cam = Camera.main.transform;
        mat = GetComponent<Renderer>().material; // instance per renderer (ok for backgrounds)
    }

    void Start()
    {
        if (cam == null)
        {
            Debug.LogError($"{name}: No camera assigned and no MainCamera found.");
            enabled = false;
            return;
        }

        startPos = transform.position;
        lastCamX = cam.position.x;

        // keep whatever offset is already in the material
        uvX = mat.GetTextureOffset(textureProperty).x;
    }

    void LateUpdate()
    {
        // 1) Plane follows camera on X only
        transform.position = new Vector3(cam.position.x + planeXOffset, startPos.y, startPos.z);

        // 2) UV scroll ONLY when camera moves (no auto drift)
        float camDeltaX = cam.position.x - lastCamX;
        camDeltaX = Mathf.Clamp(camDeltaX, -maxCamDeltaPerFrame, maxCamDeltaPerFrame);

        float dir = invert ? -1f : 1f;
        uvX += camDeltaX * uvScrollPerWorldUnitX * dir;

        // Setting only X keeps Y fixed
        Vector2 cur = mat.GetTextureOffset(textureProperty);
        cur.x = uvX;
        mat.SetTextureOffset(textureProperty, cur);

        lastCamX = cam.position.x;
    }

    public void SetTexture(Texture newTexture, bool resetOffset = true)
    {
        if (newTexture == null) return;

        // gleiche Instanz wie im Script benutzen
        mat.mainTexture = newTexture;

        if (resetOffset)
        {
            Vector2 off = mat.GetTextureOffset(textureProperty);
            off.x = 0f;
            mat.SetTextureOffset(textureProperty, off);

            // Cache + Referenzwerte sauber zurücksetzen
            uvX = 0f;
            lastCamX = cam.position.x;
        }
    }
}