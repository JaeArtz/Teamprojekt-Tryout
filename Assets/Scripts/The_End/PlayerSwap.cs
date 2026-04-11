using UnityEngine;

public class PlayerSwap : MonoBehaviour
{
    [Header("References")]
    public GameObject realPlayer;

    [Header("Settings")]
    public string animTriggerName = "LayDown";

    public void DoSwap()
    {
        if (realPlayer == null) return;

        // 1. ACTIVATE OBJECT
        gameObject.SetActive(true);

        // 2. ADAPT POSITION AND "FACING-POSITION" OF PLAYER (left or right)
        transform.position = realPlayer.transform.position;
        transform.localScale = realPlayer.transform.localScale;

        // 3. DISABLE REAL PLAYER
        realPlayer.SetActive(false);

        // 4. TRIGGER ANIMATION
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            // triggers with "LayDown"
            anim.SetTrigger(animTriggerName);
        }
        else
        {
            Debug.LogError("Kein Animator auf Animation_Sleep gefunden!");
        }
    }
}