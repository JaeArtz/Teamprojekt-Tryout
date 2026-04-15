using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    private bool wasAlreadyLoaded = false;
    private bool isFinalAnimation = false;

    public Sprite FullLifeIcon;
    public Sprite HalfLifeIcon;
    public Sprite LowLifeIcon;
    public Sprite AlmostDeadIcon;

    private RandomAudioPlayer audioPlayer;

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

            transform.position = position;
            wasAlreadyLoaded = true;
            //Player.GetComponent<PlayerRespawn>().RespawnNow();
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

        audioPlayer = GetComponents<RandomAudioPlayer>().FirstOrDefault(component => component.Name.Equals("Hurt"));

        if (!audioPlayer) Debug.LogError(@"Random Audio Player with name ""Hurt"" could not be found!");
    }

    void Start()
    {
        SaveSystem.AlterDataCheck(false);
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

        if(currentHealth == 0 && !isFinalAnimation)
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

        audioPlayer.PlayRandomSound();
    }

    void PlayFinalAnimation(int damage)
    {
        if (currentHealth - damage == 0) currentHealth = 0;
        {
            currentHealth -= damage;
            isFinalAnimation = true;
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
