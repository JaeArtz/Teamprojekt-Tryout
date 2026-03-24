using UnityEngine;

public class ShowcaseTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Prüfen, ob es der Spieler ist und ob wir schon einmal getriggert haben
        if (!hasTriggered && other.CompareTag("Player"))
        {
            // PlayerAttack Script auf dem Spieler suchen
            PlayerAttack playerAttack = other.GetComponentInParent<PlayerAttack>();

            if (playerAttack != null)
            {
                // Den Showcase direkt über eine neue public Methode starten
                playerAttack.StartManualShowcase();
                
                // Trigger deaktivieren, damit er nur einmal im Level passiert
                hasTriggered = true; 
                
                // Optional: Den Trigger-Collider ganz zerstören, um Ressourcen zu sparen
                // Destroy(gameObject); 
            }
        }
    }
}