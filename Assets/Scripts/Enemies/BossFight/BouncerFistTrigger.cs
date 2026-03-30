using UnityEngine;

public class BouncerFistTrigger : MonoBehaviour
{
    [Header("--- Setup ---")]
    [Tooltip("Ziehe hier das Objekt 'BossGolem_BouncerFist' aus der Hierarchy rein!")]
    public BouncerFist bouncerFist;

    [Tooltip("Der Tag des Players (muss exakt übereinstimmen).")]
    public string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Wir prüfen, ob das Objekt, das den Trigger berührt, den Tag "Player" hat
        if (other.CompareTag(playerTag))
        {
            if (bouncerFist != null)
            {
                // Wir geben den Befehl zum Zuschlagen
                bouncerFist.TriggerAttack();
                Debug.Log("Cheat-Schutz: Faust-Angriff wurde ausgelöst!");
            }
            else
            {
                Debug.LogWarning("BouncerFistTrigger: Keine BouncerFist zugewiesen!");
            }
        }
    }
}