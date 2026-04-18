using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    // Erstellt für BossFight: für die Boss-Fußsohle, die dem Bein folgen soll
    [Header("Einstellungen")]
    [SerializeField] private Transform target; // BossLeg
    [SerializeField] private Vector3 offset;    // Abstand zum Fuß, soll editierbar bleiben

    [Header("Zusätzliche Optionen")]
    [SerializeField] bool followX = true;
    [SerializeField] bool followY = true;

    // LateUpdate ist vermutlich besser, damit alles was in Awake und Update passiert bereits abgehandelt ist
    private void LateUpdate()
    {
        if (target == null) return;

        // Die neue Position wird berechnet basierend auf den TargetKoordinaten + dem einstellbaren Offset
        Vector3 newPosition = transform.position;

        if (followX) newPosition.x = target.position.x + offset.x;
        if (followY) newPosition.y = target.position.y + offset.y;

        transform.position = newPosition;
    }

    // HelperFunction, damit der einstellbare Wert im Inspektor funktioniert
    // Cooles Feature von Unity, mit dem man sowas direkt im Inspektor einbauen kann
    [ContextMenu("Set Current Offset")]
    private void SetCurrentOffset()
    {
        if (target != null)
            offset = transform.position - target.position;
    }
}