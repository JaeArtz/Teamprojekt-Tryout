using UnityEngine;

public class PlayerSwap : MonoBehaviour
{
    [Header("Referenzen")]
    public GameObject realPlayer;

    [Header("Einstellungen")]
    public string animTriggerName = "LayDown";

    public void DoSwap()
    {
        if (realPlayer == null) return;

        // 1. Das Objekt aktivieren
        gameObject.SetActive(true);

        // 2. Position und Blickrichtung vom Player übernehmen
        transform.position = realPlayer.transform.position;
        transform.localScale = realPlayer.transform.localScale;

        // 3. Echten Player ausschalten
        realPlayer.SetActive(false);

        // 4. DIE ANIMATION TRIGGERN
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            // Hier wird der Name aus dem Inspector ("LayDown") benutzt!
            anim.SetTrigger(animTriggerName);
        }
        else
        {
            Debug.LogError("Kein Animator auf Animation_Sleep gefunden!");
        }
    }
}