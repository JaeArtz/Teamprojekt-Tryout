using System.Collections;
using UnityEngine;

public class GolemBoss : MonoBehaviour
{
    public enum GolemState { Sleeping, Awakening, Attacking, Defeated }
    public GolemState currentState = GolemState.Sleeping;

    [Header("Settings")]
    public float attackDelay = 2f;
    public GameObject groundWavePrefab;
    public GameObject fistShadowPrefab;
    public Transform leftFootPos, rightFootPos, fistTargetPos;

    public void WakeUp()
    {
        currentState = GolemState.Awakening;
        StartCoroutine(BossLoop());
    }

    private IEnumerator BossLoop()
    {
        // 1. Transformation Animation (Platzhalter)
        yield return new WaitForSeconds(3f);
        currentState = GolemState.Attacking;

        while (currentState == GolemState.Attacking)
        {
            // RHYTHMUS: 2x Linker Fuß -> 2x Faust -> 1x Rechter Fuß (Lücke) -> 2x Faust

            // 2x Linker Fuß
            for (int i = 0; i < 2; i++) { yield return Stamp(leftFootPos); }

            // 2x Faust
            for (int i = 0; i < 2; i++) { yield return FistSlam(); }

            // 1x Rechter Fuß (Hier kann der Spieler vorbei!)
            yield return Stamp(rightFootPos, isPassageChance: true);

            // 2x Faust
            for (int i = 0; i < 2; i++) { yield return FistSlam(); }
        }
    }

    private IEnumerator Stamp(Transform pos, bool isPassageChance = false)
    {
        Debug.Log("Golem hebt Fuß...");
        yield return new WaitForSeconds(1f); // Zeit zum Drunterherrennen, wenn es der rechte Fuß ist

        Debug.Log("STAMPF!");
        // Hier Instanziieren der Welle (Nutzt dein Projectile/EnemyDamage Prinzip)
        Instantiate(groundWavePrefab, pos.position, Quaternion.identity);
        yield return new WaitForSeconds(attackDelay);
    }

    private IEnumerator FistSlam()
    {
        // Schatten anzeigen
        GameObject shadow = Instantiate(fistShadowPrefab, fistTargetPos.position, Quaternion.identity);
        yield return new WaitForSeconds(1.5f);

        // Einschlag (Schaden)
        Debug.Log("FAUSTSCHLAG!");
        Destroy(shadow);
        // Hier Damage-Area kurz aktivieren
        yield return new WaitForSeconds(attackDelay);
    }
}