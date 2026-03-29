using System;
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
        // 1. Transformation Animation (PlaceHolder)
        yield return new WaitForSeconds(3f);
        currentState = GolemState.Attacking;

        while (currentState == GolemState.Attacking)
        {
            // RHYTH: 2x Left Foot Stomp -> 2x PunchFloor -> 1x Right Foot Stomp -> 2x PunchFloor

            // 2x Left Foot, LF also needs is‹assegeChance
            for (int i = 0; i < 2; i++) { yield return Stamp(leftFootPos); }

            // 2x Punch
            for (int i = 0; i < 2; i++) { yield return FistSlam(); }

            // 1x Right Foot
            yield return Stamp(rightFootPos, isPassageChance: true);

            // 2x Punch
            for (int i = 0; i < 2; i++) { yield return FistSlam(); }
        }
    }

    private IEnumerator Stamp(Transform pos, bool isPassageChance = false)
    {
        Debug.Log("Golem hebt Fuﬂ...");
        yield return new WaitForSeconds(1f); 

        Debug.Log("STOMP!");
        // Instantiate Wave with Stomp
        Instantiate(groundWavePrefab, pos.position, Quaternion.identity);
        yield return new WaitForSeconds(attackDelay);
    }

    private IEnumerator FistSlam()
    {
        // Cast Shadow before Punch on Floor
        GameObject shadow = Instantiate(fistShadowPrefab, fistTargetPos.position, Quaternion.identity);
        yield return new WaitForSeconds(1.5f);

        Debug.Log("PUNCH!");
        Destroy(shadow);
        
        yield return new WaitForSeconds(attackDelay);
    }

    internal void StartBossFight()
    {
        throw new NotImplementedException();
    }
}