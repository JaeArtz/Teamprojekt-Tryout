using System.Collections;
using UnityEngine;

public class LastBreath : MonoBehaviour
{
    private PlayerHealth player;
    private GameObject HUD;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HUD = GameObject.Find("HUDCanvas");
        player = GameObject.Find("Player").GetComponent<PlayerHealth>();
    }

    public IEnumerator LastDyingBreath()
    {
        float WaitforLiveTick = 9f / player.currentHealth;
        while (player.currentHealth > 0)
        {
            yield return new WaitForSeconds(WaitforLiveTick);
            player.GetComponent<PlayerHealth>().PlayFinalAnimation(1);
        }
        HUD.SetActive(false);
        
        
    }
}
