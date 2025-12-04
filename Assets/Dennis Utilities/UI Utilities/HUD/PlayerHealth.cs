using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Assertions;
public class PlayerHealth : MonoBehaviour
{
    public GameObject Player;
    public static event Action OnPlayerDamaged;
    public static event Action OnPlayerDeath;

    public GameObject myLevelLoader;

    public int maxHealth;
    public int currentHealth;

    public Image playerIcon;

    public Sprite FullLifeIcon;
    public Sprite HalfLifeIcon;
    public Sprite LowLifeIcon;
    public Sprite AlmostDeadIcon;


    // Start is called before the first frame update
    void Start()
    {
        Assert.AreEqual(currentHealth, maxHealth);
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.O))
        {
            TakeDamage(1);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            GainHealth(1);
        }*/

        EvaluatePlayerIconAppearance();

        if(currentHealth == 0)
        {
            Player.SetActive(false);
            myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene("GameOverScreen");
        }
    }

    //Testfunction, might be optimized and changed later on!
    private void EvaluatePlayerIconAppearance()
    {
        if (currentHealth < 2)
        {
            playerIcon.sprite = AlmostDeadIcon;
        }
        else if (currentHealth < 3)//TODO: make range last from 6% to 33%
        {
            playerIcon.sprite = LowLifeIcon;
        }
        else if (currentHealth < 5) //TODO: make range last from 33% to 66%
        {
            playerIcon.sprite = HalfLifeIcon;
        }
        else //TODO: make range last from 66% to 100%
        {
            playerIcon.sprite = FullLifeIcon;
        }

    }

    public void TakeDamage(int damage)
    {
        if((currentHealth - damage >= 0))
        {
            currentHealth -= damage;
            //healthBar.SetHealth(currentHealth);
            OnPlayerDamaged?.Invoke();
        }
        else
        {
            currentHealth = 0;

        }
        
    }

    void GainHealth(int health)
    {
        if (!(currentHealth >= maxHealth))
        {
            currentHealth += health;
            //healthBar.SetHealth(currentHealth);
            OnPlayerDamaged?.Invoke();
        }
    }
     
}
