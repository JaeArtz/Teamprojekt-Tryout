using UnityEngine;

public class Ghost : MonoBehaviour
{
    #region Hit Cooldown
    private const float cooldown = 1.0f;
    private float lastCooldownTime = 0.0f;
    bool cooldownActive = false;
    bool isInEnemy = false;
    Collider2D collides;
    #endregion

    #region animation
    private Vector3 startingPosition =new Vector3(232.0f,4.0f,0.0f);
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
        transform.position = startingPosition + Vector3.up * Mathf.Sin(Time.realtimeSinceStartup) * 0.5f;
        if (state == GhostState.Stunned)
        {
            stunTimer -= Time.deltaTime;

            if (stunTimer <= 0f)
                SetActive();
        }
        
    }

    void FixedUpdate()
    {

        if (Time.time - lastCooldownTime >=  cooldown)
        {
            //add the current time to the damage delay
            //lastCooldownTime = Time.time;
            cooldownActive = false;
        }
        if (isInEnemy)
        {
            if (!cooldownActive)
            {
                cooldownActive = true;
                lastCooldownTime = Time.time;
                Debug.Log("Player nimmt Schaden!");
                collides.GetComponent<PlayerHealth>().TakeDamage(1);
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
            isInEnemy = false;
            return;
        }

        if (other.CompareTag("Player") )
        {
            isInEnemy = true;
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
