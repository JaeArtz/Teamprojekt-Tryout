using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float stunDuration = 2f;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Geist ist harmlos?
        if (state == GhostState.Stunned) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player nimmt Schaden!");
            // oder: other.GetComponent<Player>().TakeDamage();
        }
    }
}

public enum GhostState { Active, Stunned }
