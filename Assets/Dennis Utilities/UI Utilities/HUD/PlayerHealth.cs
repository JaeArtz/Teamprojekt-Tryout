using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Assertions;
public class PlayerHealth : MonoBehaviour
{
    private GameObject Player;
    public static event Action OnPlayerDamaged;
    public static event Action OnPlayerDeath;
    public static event Action OnPlayerLiveUpdate;

    private GameObject myLevelLoader;
    private LevelManager myLevelManager;

    public int maxHealth;
    public int currentHealth;
    public string currentScene;
    private Image playerIcon;

    public Sprite FullLifeIcon;
    public Sprite HalfLifeIcon;
    public Sprite LowLifeIcon;
    public Sprite AlmostDeadIcon;


    private void Awake()
    {
        playerIcon = GameObject.Find("PlayerHeadIcon").GetComponent<Image>();
        myLevelLoader = GameObject.Find("LevelLoader");
        myLevelManager = GameObject.Find("GameManager").GetComponent<LevelManager>();
        Player = GameObject.Find("Player");

        PlayerData data = SaveSystem.LoadData();
        if(data != null && data.wasLoaded == true)
        {
            currentHealth = data.currentLives;
            maxHealth = 6 + SaveSystem.LoadLeafData().Count;
            
            Vector3 position;
            position.x = data.currentPosition[0];
            position.y = data.currentPosition[1];
            position.z = data.currentPosition[2];

            transform.localPosition = position;
            SaveSystem.AlterDataCheck(false);
        }
        else
        {
            if (!(SaveSystem.LoadData() == null))
                maxHealth = 6 + SaveSystem.LoadLeafData().Count;
            else
                maxHealth = 6;
            currentHealth = maxHealth;
        }
        currentScene = SceneManager.GetActiveScene().name;
    }

    void Start()
    {
        myLevelManager.SaveCurrentLevel();
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
            //Player.SetActive(false);
            //myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene("GameOverScreen");
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        // HP zurücksetzen
        currentHealth = maxHealth;

        // Zum Checkpoint teleportieren via PlayerRespawn
        PlayerRespawn playerRespawn = Player.GetComponentInParent<PlayerRespawn>();
        if (playerRespawn != null)
        {
            playerRespawn.RespawnNow();
        }

        // Spieler wieder aktivieren (falls deaktiviert)
        Player.SetActive(true);

        OnPlayerDamaged?.Invoke();
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
        //Debug.Log("TakeDamage aufgerufen, HP vorher: " + currentHealth); //zum testen
        
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

    public void UpdateLives()
    {
        maxHealth = 6 + SaveSystem.LoadLeafData().Count;
        if (!(currentHealth >= maxHealth))
            currentHealth++;
        //healthBar.SetHealth(currentHealth);
        OnPlayerLiveUpdate?.Invoke();
        
    }

}
