using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WindBaseSpeedSlider : MonoBehaviour
{
    [Range(0f, 1f)]
    [Tooltip("Scales world wind values from WindController for this object only.")]
    public float windScale = 0.5f;

    public string influenceProperty = "_Externalinfluence";
    public string speedProperty = "_WindSpeed";

    [Tooltip("Optional: assign explicitly. If null, then WindController.Instance is used at runtime.")]
    public WindController windController;

    SpriteRenderer sr;
    MaterialPropertyBlock mpb;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
    }

    void LateUpdate()
    {
        
        var wc = windController ? windController : WindController.Instance;
        if (!wc) return;

        // preserves any other properties that are already on the renderer
        sr.GetPropertyBlock(mpb);

        // always scales from the controller's current world wind values
        // I had trouble there before, this is the fix for it
        float inf = wc.CurrentInfluence * windScale;
        float spd = wc.CurrentSpeed * windScale;

        mpb.SetFloat(influenceProperty, inf);
        mpb.SetFloat(speedProperty, spd);

        sr.SetPropertyBlock(mpb);
    }
}
