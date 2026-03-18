using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WindGustSmallGrass : MonoBehaviour
{
    [Header("WindGust Setting for Small Grass")]
    [Range(0f, 0.5f)]
    [Tooltip("Sets how far the Sprite can be bent")]
    public float maxStretchLimit = 0.05f;

    [Tooltip("Referenzname im Shader (dein ObjectScaleCompensation)")]
    public string objectScaleCompensation = "_ObjectScaleCompensation";

    private SpriteRenderer sr;
    private MaterialPropertyBlock mpb;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
    }

    void LateUpdate()
    {
        if (WindController.Instance == null) return;

        // gets current propertyblock, values are not overwritten
        sr.GetPropertyBlock(mpb);

        // with this I am trying to reduce the stre´ngth of the windgust-influence...
        float finalAmplitude = WindController.Instance.CurrentInfluence * maxStretchLimit;

        // sends new value to shader, to reduce "stretching" (hopefully)
        mpb.SetFloat(objectScaleCompensation, finalAmplitude);

        sr.SetPropertyBlock(mpb);
    }
}