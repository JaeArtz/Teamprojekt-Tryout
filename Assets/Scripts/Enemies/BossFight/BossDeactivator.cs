using UnityEngine;

public class BossDeactivator : MonoBehaviour
{
    [Header("Referenzen")]
    public GameObject bossFightContainer; // Der BossCombatManager
    public GameObject golemDebris;       // Die Trümmer

    // Diese Funktion verknüpfst du im Unity-Event deiner Rune
    public void OnRuneActivated()
    {
        // 1. Boss stoppen
        if (bossFightContainer != null) bossFightContainer.SetActive(false);

        // 2. Trümmer an
        if (golemDebris != null) golemDebris.SetActive(true);

        Debug.Log("Boss durch Rune deaktiviert!");
    }
}