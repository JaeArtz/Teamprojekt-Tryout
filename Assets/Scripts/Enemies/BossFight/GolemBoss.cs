using System;
using System.Collections;
using UnityEngine;

public class GolemBoss : MonoBehaviour
{
    public enum GolemState { Sleeping, Awakening, Attacking, Defeated }
    public GolemState currentState = GolemState.Sleeping;

    [Header("Settings")]
    [Tooltip("Delay after each Stomp and Punch")]
    public float attackDelay = 2f;
    [Tooltip("Drag ShockWave Prefab in here")]
    public GameObject groundWavePrefab;
    [Tooltip("Drag Shadow Sprite Prefab in here")]
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
            // RHYTHM: 2x Left Foot Stomp -> 2x PunchFloor -> 1x Right Foot Stomp -> 2x PunchFloor

            // 2x Left Foot
            for (int i = 0; i < 2; i++) { yield return Stomp(leftFootPos); }

            // 2x Punch
            for (int i = 0; i < 2; i++) { yield return FistSlam(); }

            // 1x Right Foot
            yield return Stomp(rightFootPos, isPassageChance: true);

            // 2x Punch
            for (int i = 0; i < 2; i++) { yield return FistSlam(); }
        }
    }

    private IEnumerator Stomp(Transform pos, bool isPassageChance = false)
    {
        Debug.Log("Golem raises Foot...");
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