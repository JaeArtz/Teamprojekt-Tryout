using UnityEngine;

public class PlayerWallActions : MonoBehaviour
{
    [SerializeField, Tooltip("The (absolute value) horizontal velocity being applied when the player enters wall-jump")] private float playerMaxWallJumpVelocityX = 18f;
    public float PlayerMaxWallJumpVelocityX => playerMaxWallJumpVelocityX;
    [SerializeField, Tooltip("The (absolute value) vertical velocity being applied when the player enters wall-jump")] private float playerMaxWallJumpVelocityY = 12f;
    [SerializeField, Tooltip("The maximum fall speed, the player can reach while wall-sliding")] private float wallSlideSpeed = -3f;
    
    [SerializeField, Tooltip("Cooldown being triggered when entering wall-jump. Disables the check of player touching wall.")] private float wallJumpCooldownDuration = 0.2f;
    private float wallJumpCooldownTimer;

    [SerializeField, Tooltip("Duration in seconds until the player control is back to normal. Player control gets interpolated.")]
    private float wallJumpAirControlDuration = 1.0f;
    private float wallJumpAirControlTimer;

    [SerializeField, Tooltip("Duration in seconds until the player is allowed to roll again after walljumping.")]
    private float prohibitRollDuration = 1.0f;
    private float prohibitRollTimer;
    public bool CanRoll => prohibitRollTimer <= 0;

    [SerializeField, Tooltip("How long the player can still perform normal jump when exiting ground")] private float wallCoyoteTime = 0.15f;
    private float wallCoyoteTimer;

    [Header("Collider Settings")]
    [SerializeField, Tooltip("How far the cast start is away from the player collider to check for a nearby wall")]
    private float wallCheckDistance = 0.1f;
    [SerializeField, Tooltip("Circle collider without collision for wall check")]
    private CircleCollider2D wallTrigger;


    private Rigidbody2D body;
    public Rigidbody2D Body { set { body = value; } }

    public float T => wallJumpAirControlTimer / wallJumpAirControlDuration;

    private bool _isGrounded;
    private bool _isOnWall;
    private bool _isWallJumping;
    private bool _isDetached;
    private bool _isWallSliding;
    private int _playerWallDirection;
    private int _lastWallDirection;

    public void Awake()
    {
        if (wallTrigger == null)
        {
            // 1. Erst am eigenen GameObject suchen
            wallTrigger = GetComponent<CircleCollider2D>();

            // 2. Falls nicht gefunden, in Children suchen
            if (wallTrigger == null)
            {
                wallTrigger = GetComponentInChildren<CircleCollider2D>();

                if (wallTrigger != null)
                {
                    Debug.Log($"CircleCollider2D gefunden in Child: {wallTrigger.gameObject.name}");
                }
            }
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetWallJumpAirControlDuration()
    {
        wallJumpAirControlTimer = 0;
    }

    private bool CanWallJump()
    {
        return !_isWallJumping && (_isOnWall || wallCoyoteTimer > 0) && wallJumpCooldownTimer <= 0;
    }

    public bool HandleWallActions(ref LayerMask layer, bool isGrounded)
    {
        _isGrounded = isGrounded;
        if (!_isOnWall && wallCoyoteTimer > 0)
            wallCoyoteTimer -= Time.fixedDeltaTime;

        if (_isOnWall)
            wallCoyoteTimer = wallCoyoteTime;

        if(isGrounded)
            wallJumpAirControlTimer = 0;

        if (!Input.GetKeyDown(KeyCode.Space))
            return false;


        if (!CanWallJump())
            return false;

        WallJump();
        _isWallJumping = true;

        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;
        if (wallJumpCooldownTimer > 0) return;

        if (other.CompareTag("Wall"))
        {
            _isOnWall = true;
            _isWallJumping = false;
            Vector2 contactPoint = other.ClosestPoint(transform.position);
            Vector2 direction = (Vector2)transform.position - contactPoint;
            direction.Normalize();
            _playerWallDirection = direction.x > 0 ? -1 : 1;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == null) return;
        if (wallJumpCooldownTimer > 0) return;

        if (other.CompareTag("Wall"))
        {
            _isOnWall = false;
        }
    }

    public bool HandleFixedActions()
    {
        if (!body) return false;

        if (wallJumpAirControlTimer > 0)
            wallJumpAirControlTimer -= Time.fixedDeltaTime;

        if (wallJumpCooldownTimer > 0)
            wallJumpCooldownTimer -= Time.fixedDeltaTime;

        if (prohibitRollTimer > 0)
            prohibitRollTimer -= Time.fixedDeltaTime;

        // ZUSTANDS-ÜBERPRÜFUNG WALL
        if (wallJumpCooldownTimer <= 0)
        {
            if (_isOnWall)
                _lastWallDirection = _playerWallDirection;
        }

        // WANDGLEITEN
        if (_isOnWall && !_isGrounded && Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f)
            if (Mathf.Sign(Input.GetAxisRaw("Horizontal")) == _playerWallDirection)
            {
                _isWallSliding = true;
                if (body.linearVelocity.y < wallSlideSpeed)
                    body.linearVelocity = new Vector2(body.linearVelocity.x, wallSlideSpeed);

                wallCoyoteTimer = wallCoyoteTime;

                body.linearVelocity = new Vector2(0, body.linearVelocity.y);
            }
            else
            {
                _isWallSliding = false;
            }

        return _isWallSliding;
    }

    private void WallJump()
    {
        if (!body) return;

        int wallDir = _isOnWall ? _playerWallDirection : _lastWallDirection;
        float jumpDirX = -wallDir;

        Vector2 jumpForce = new Vector2(
            jumpDirX * playerMaxWallJumpVelocityX,
            playerMaxWallJumpVelocityY
        );
        body.linearVelocity = jumpForce;

        _isWallJumping = true;
        wallJumpAirControlTimer = wallJumpAirControlDuration;
        wallJumpCooldownTimer = wallJumpCooldownDuration;
        prohibitRollTimer = prohibitRollDuration;

        _isOnWall = false;
        _isWallSliding = false;
        wallCoyoteTimer = 0;
        _isDetached = true;
    }
}
