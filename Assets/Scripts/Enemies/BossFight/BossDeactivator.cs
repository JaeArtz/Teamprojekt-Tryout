using UnityEngine;

public class BossDeactivator : MonoBehaviour
{
    [Header("Referenzen")]
    public GameObject bossFightContainer; // Contains BossCombatManager
    public GameObject golemDebris;       // Debris to spawn after Defeating Boss

    // connect this to mainRuneStone in Scene
    public void OnRuneActivated()
    {
        // 1. Boss STOP
        if (bossFightContainer != null) bossFightContainer.SetActive(false);

        // 2. Debris ON
        if (golemDebris != null) golemDebris.SetActive(true);

        Debug.Log("Boss was deactivated by Rune!");
    }
}