using System.Collections;
using UnityEngine;

public class RuneManager : MonoBehaviour
{
    public int requiredRunes = 3;
    private int currentActiveRunes = 0;

    [Header("Events")]
    public CheckpointTrigger bossStartTrigger; // Der Trigger, der die Erwachen-Sequenz startet
    public GameObject victoryRune; // Die Rune auf der rechten Seite

    public void ActivateRune()
    {
        currentActiveRunes++;
        Debug.Log($"Rune aktiviert! ({currentActiveRunes}/{requiredRunes})");

        if (currentActiveRunes >= requiredRunes)
        {
            StartCoroutine(TriggerBossAwakening());
        }
    }

    private IEnumerator TriggerBossAwakening()
    {
        yield return new WaitForSeconds(0.5f);
        if (bossStartTrigger != null)
        {
            // Wir nutzen dein vorhandenes System, um die Sequenz zu starten
            bossStartTrigger.enabled = true;
            // Falls der Trigger normalerweise auf Kollision wartet, 
            // rufen wir hier manuell die Logik auf oder setzen den Player davor.
        }
    }
}