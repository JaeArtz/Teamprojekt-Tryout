using UnityEngine;

public class Ghost : MonoBehaviour
{
    #region Hit Cooldown
    public const float cooldown = 2.0f;
    public float lastCooldownTime = 0.0f;
    bool cooldownActive = false;
    bool isInEnemy = false;
    Collider2D collides;
    #endregion

    public float stunDuration = 4f;
    public Sprite normalSprite;
    public Sprite stunnedSprite;

    private GhostState state = GhostState.Active;
    private float stunTimer;
    private SpriteRenderer sr;

    [SerializeField] private CircleCollider2D hitCollider;
    [SerializeField] private BoxCollider2D bodyCollider;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (state == GhostState.Stunned)
        {
            stunTimer -= Time.deltaTime;

            if (stunTimer <= 0f)
                SetActive();
        }

        if (Time.time >= lastCooldownTime)
        {
            //add the current time to the damage delay
            lastCooldownTime = Time.time + cooldown;
            cooldownActive = false;       
        }

        if(isInEnemy)
        {
            if (!cooldownActive)
            {
                Debug.Log("Player nimmt Schaden!");
                collides.GetComponent<PlayerHealth>().TakeDamage(1);
                cooldownActive = true;
            }
        }
    }

    private void SetStunned()
    {
        state = GhostState.Stunned;
        stunTimer = stunDuration;
        sr.sprite = stunnedSprite;

        bodyCollider.enabled = false;
    }

    private void SetActive()
    {
        state = GhostState.Active;
        sr.sprite = normalSprite;

        bodyCollider.enabled = true;
    }

    public void HitByLight()
    {
        SetStunned();
    }

    /// <summary>
    /// Damages the Player when cooldown is Inactive by Collision and when the Ghost is not stunned.
    /// </summary>
    /// <param name="other">Collider which is colliding with the Ghost.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Geist ist harmlos?
        if (state == GhostState.Stunned)
        {
            //isInEnemy = false;
            return;
        }
        isInEnemy = true;
        if (other.CompareTag("Player") && !cooldownActive)
        {
            collides = other;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        
        isInEnemy = false;
        // Geist ist harmlos?
        if (state == GhostState.Stunned) 
        {
            //isInEnemy = false; 
            return; 
        }
    }




}




public enum GhostState { Active, Stunned }
