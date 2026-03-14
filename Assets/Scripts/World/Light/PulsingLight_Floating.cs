using UnityEngine;

[RequireComponent(typeof(Light))]
public class PulsingLight_Floating : MonoBehaviour
{
    [Header("Light Settings")]
    [Tooltip("Assign lightSource, or use default light")]
    public Light lightSource;
    [Tooltip("Minimum Brightness if Light")]
    public float minIntensity = 0.5f;
    [Tooltip("Maximum Brightness if Light")]
    public float maxIntensity = 2f;
    [Tooltip("Influence value for sinus calculation of PulseFrequency")]
    public float PulseFrequency = 2f;

    private void Awake()
    {
        if (lightSource == null) { lightSource = GetComponent<Light>(); }
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * PulseFrequency) + 1f) / 2f;
        lightSource.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
    }
}
