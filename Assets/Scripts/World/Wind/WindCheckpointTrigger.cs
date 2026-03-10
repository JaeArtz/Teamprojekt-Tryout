using UnityEngine;

public class WindCheckpointTrigger : MonoBehaviour
{
    public WindController windController;

    [Header("WindGust Settings")]
    public float gustDuration = 7f;
    public float gustInfluence = 1.5f;
    public float gustSpeed = 3f;

    [Header("Trigger Settings")]
    public bool triggerOnlyOnce = true;
    bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnlyOnce && hasTriggered) return;
        if (!other.transform.root.CompareTag("Player")) return;

        hasTriggered = true;

        if (windController)
            windController.TriggerGust(gustDuration, gustInfluence, gustSpeed, true);
    }
}
