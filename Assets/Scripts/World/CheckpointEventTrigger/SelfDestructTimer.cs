using UnityEngine;

public class SelfDestructTimer : MonoBehaviour
{
    [Header("Time until Death")]
    [Tooltip("Counts this amount of seconds down starting from activation of object until self destruct")]
    public float lifetime = 20f;

    void Start()
    {        
        Destroy(gameObject, lifetime);
    }
}