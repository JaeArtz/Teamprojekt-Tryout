using System.Collections.Generic;
using UnityEngine;

public class HealthHeartBar : MonoBehaviour
{
    public GameObject heartPrefab;

    public PlayerHealth playerHealth;

    List<HealthHeart> hearts = new List<HealthHeart>();

    private void Start()
    {
        DrawHearts();
    }

    public void OnEnable()
    {
        PlayerHealth.OnPlayerDamaged += DrawHearts;
    }

    public void OnDisable()
    {
        PlayerHealth.OnPlayerDamaged -= DrawHearts;
    }

    public void DrawHearts()
    {
        ClearHearts();

        //determine amount of hearts to make total
        //based off max Health

        float maxHealthRemainder = playerHealth.maxHealth % 2;
        int heartsToMake = (int)(playerHealth.maxHealth / 2 + maxHealthRemainder);

        for (int i = 0; i < heartsToMake; i++)
        {
            CreateEmptyHeart();
        }
        for (int i = 0; i < hearts.Count; i++)
        {
            int heartStatusRemainder = (int)Mathf.Clamp(playerHealth.currentHealth - (i * 2), 0, 2);
            hearts[i].SetHeartImage((HeartStatus)heartStatusRemainder);
        }
    }

    public void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(heartPrefab);
        newHeart.transform.SetParent(transform);
        
        HealthHeart heartComponent = newHeart.GetComponent<HealthHeart>();
        heartComponent.SetHeartImage(HeartStatus.Empty);
        hearts.Add(heartComponent);
    }

    public void ClearHearts()
    {
        foreach(Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        hearts = new List<HealthHeart>();
    }
}
