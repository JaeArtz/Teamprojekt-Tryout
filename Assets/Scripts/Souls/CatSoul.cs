using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CatSoul : MonoBehaviour
{
    [SerializeField] private Light2D playerLight;
    [SerializeField] private float lightRadius = 3f;
    [SerializeField] private float lightIntensity = 1f;


    public void ActivateLight()
    {
        playerLight.pointLightOuterRadius = lightRadius;
        playerLight.intensity = lightIntensity;
        playerLight.enabled = true;
    }
}
