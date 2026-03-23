using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class PlayerLight : MonoBehaviour
{
    [SerializeField] private Light2D playerLight;

    private SoulManager soulManager;

    private void Awake()
    {
        soulManager = GameObject.Find("GameManager").GetComponent<SoulManager>();
    }
    private void Start()
    {
        bool hasLight = soulManager.HasSoul("catSoul");
        playerLight.gameObject.SetActive(true);
        playerLight.enabled = hasLight;
    }

    public void ActivateLight()
    {
        Debug.Log("CatSoul collected: Activating player light.");
        playerLight.gameObject.SetActive(true);
        playerLight.enabled = true;
        Debug.Log("Player light enabled: " + playerLight.enabled);
    }

}