using System.Collections;
using UnityEngine;

public class RuneManager : MonoBehaviour
{
    public int requiredRunes = 3;
    private int currentActiveRunes = 0;

    [Header("Events")]
    public CheckpointTrigger bossStartTrigger;
    public GameObject victoryRune; // Rune on RightSide, put Golem to sleep

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
            
            bossStartTrigger.enabled = true;
            
        }
    }
}