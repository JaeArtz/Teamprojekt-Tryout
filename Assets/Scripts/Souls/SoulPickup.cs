using UnityEngine;
using System.Collections;

public class SoulPickup : MonoBehaviour
{
    [Header("Daten")]
    [Tooltip("Zuweisung des ScriptableObjects, das die ID und Infos dieser Seele enthält")]
    public SoulData soulData;

    [Header("UI Feedback")]
    [Tooltip("Optional: Ein UI-Element (z.B. Textfeld), das beim Einsammeln erscheint")]
    public GameObject dialogBox;
    public float dialogDuration = 3f; // Wie lange die Nachricht sichtbar bleibt

    private bool collected = false; // Verhindert Mehrfacheinsammlung in einem Frame

    private void Start()
    {
        // UI zu Beginn sicherheitshalber ausblenden
        if (dialogBox != null) dialogBox.SetActive(false);

        // Prüfen, ob der Spieler diese Seele bereits in einem früheren Spielstand gesammelt hat
        if (SoulManager.Instance != null && SoulManager.Instance.HasSoul(soulData.soulID))
        {
            // Falls ja, wird das Pickup sofort deaktiviert, damit es nicht doppelt erscheint
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Nur einmal sammeln und nur durch den Spieler
        if (collected) return;
        if (!other.CompareTag("Player")) return;
        
        collected = true;

        // 1. Im SoulManager registrieren (für dauerhafte Speicherung/PlayerPrefs)
        SoulManager.Instance?.AddSoul(soulData.soulID);

        // 2. PlayerMovement informieren (für die Freischaltung des Doppelsprungs)
        var pm = other.GetComponentInParent<PlayerMovement>();
        if (pm != null)
        {
            pm.OnSoulCollected(soulData);
        }

        // 3. PlayerAttack informieren (für die Freischaltung des Lichtschusses)
        var pa = other.GetComponentInParent<PlayerAttack>();
        if (pa != null)
        {
            pa.OnSoulCollected(soulData);
        }

        // 4. Optionales UI-Feedback anzeigen
        if (dialogBox != null)
        {
            dialogBox.SetActive(true);
            // Coroutine starten, um das UI nach X Sekunden wieder zu schließen
            StartCoroutine(HideDialogAfterDelay(dialogDuration));
        }

        // 5. Das Objekt aus der Szene entfernen (deaktivieren)
        // Hinweis: Wir nutzen SetActive(false), da wir evtl. noch die Coroutine für den Dialog brauchen
        gameObject.SetActive(false);
    }

    // Blendet den Dialog nach einer bestimmten Zeit wieder aus
    private IEnumerator HideDialogAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (dialogBox != null) dialogBox.SetActive(false);
    }
}