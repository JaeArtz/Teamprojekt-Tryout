using UnityEngine;

public class PulsingLight_Floating : MonoBehaviour
{
    public Light lightSource;
    public float minIntensity = 0.5f;
    public float maxIntensity = 2f;
    public float PulseFrequency = 2f;

    void Update()
    {
        float t = (Mathf.Sin(Time.time * PulseFrequency) + 1f) / 2f;
        lightSource.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
    }
}
