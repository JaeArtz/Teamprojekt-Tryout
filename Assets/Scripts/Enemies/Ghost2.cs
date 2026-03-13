using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Ghost2 : MonoBehaviour
{
    #region Hit Cooldown
    private const float cooldown = 1.0f;
    private float lastCooldownTime = 0.0f;
    bool cooldownActive = false;
    bool isInEnemy = false;

    // Statt Collider2D speichern wir direkt die PlayerHealth-Referenz
    private PlayerHealth playerHealth;
    #endregion

    #region animation
    private Vector3 startingPosition;
    private Vector3 lastPosition;
    #endregion

    [Header("Sprite Settings")]
    public float stunDuration = 4f;
    public Sprite normalSprite;
    public Sprite stunnedSprite;

    [Header("Animation Settings")]
    [Tooltip("if ghost constantly hovers up and down on one spot, he cannot use the GhostLight_Path Script")]
    public bool useInternalHover = true; // can be disabled in Inspector
    public float hoverAmplitude = 0.5f;
    public float hoverSpeed = 2f;
    [Tooltip("Decides wether or not Sprite is mirrored while moving from left to right/ right to lef")]
    public bool autoFlipp = true;
    [Tooltip("Does the original Sprite Image used in the spriteRenderer face right?")]
    public bool spriteFacesRightByDefault = false;
    

    private GhostState state = GhostState.Active;
    private float stunTimer;
    private SpriteRenderer sr;

    [SerializeField] private CircleCollider2D hitCollider;
    [SerializeField] private BoxCollider2D bodyCollider;


    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        startingPosition = transform.position;
        lastPosition = transform.position;
    }

    void Update()
    {
        if (useInternalHover)
        {
            transform.position = startingPosition + Vector3.up * Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
        }

        if (state == GhostState.Stunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f) SetActive();
        }


        if (state == GhostState.Stunned)
        {
            stunTimer -= Time.deltaTime;

            if (stunTimer <= 0f)
                SetActive();
        }

        if (autoFlipp && sr != null)
        {
            float xFactorDifference = transform.position.x - lastPosition.x;

            // means Sprite is moving towards the right
            if (xFactorDifference > 0.001f)
            {
                sr.flipX = spriteFacesRightByDefault ? false : true;
            }
            // means Sprite is moving towards the left
            else if (xFactorDifference < -0.001f)
            { 
                sr.flipX = spriteFacesRightByDefault ? true : false;
            }
        }
        
    }

    private void LateUpdate()
    {
        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        // Cooldown ablaufen lassen
        if (Time.time - lastCooldownTime >= cooldown)
        {
            cooldownActive = false;
        }

        if (isInEnemy && !cooldownActive)
        {
            if (playerHealth != null)
            {
                cooldownActive = true;
                lastCooldownTime = Time.time;
                //Debug.Log("Player nimmt Schaden! (Ghost2 -> TakeDamage)");
                playerHealth.TakeDamage(1);
            }
            else
            {
                // Defensive: falls referenz verloren ging
                Debug.LogWarning("PlayerHealth Referenz fehlt beim Schaden. OnTriggerEnter hat vermutlich keinen PlayerHealth gefunden.");
                isInEnemy = false;
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
    /// Damages the Player when cooldown is inactive by collision and when the Ghost2 is not stunned.
    /// </summary>
    /// <param name="other">Collider which is colliding with the Ghost2.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Geist ist harmlos?
        if (state == GhostState.Stunned)
        {
            isInEnemy = false;
            return;
        }

        if (other.CompareTag("Player"))
        {
            // Suche direkt die PlayerHealth im Parent-Hierarchie
            playerHealth = other.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                isInEnemy = true;
            }
            else
            {
                Debug.LogWarning("OnTriggerEnter2D: Collider hat Tag 'Player', aber kein PlayerHealth in Parent gefunden. Collider: " + other.name);
                isInEnemy = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Wenn Player rausgeht, resetten
        if (other.CompareTag("Player"))
        {
            isInEnemy = false;
            playerHealth = null;
        }

        // Geist ist harmlos?
        if (state == GhostState.Stunned)
        {
            return;
        }
    }
}

