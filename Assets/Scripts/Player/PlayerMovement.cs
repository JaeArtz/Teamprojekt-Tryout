using System;
using TMPro;
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Tracker")]
    [SerializeField] private float t_movementX;
    [SerializeField] private float t_movementY;
    [SerializeField] private float t_movementDirection;
    [SerializeField] private float t_groundCoyoteCounter;
    [SerializeField] private float t_wallCoyoteCounter;
    [SerializeField] private bool t_isGrounded;
    [SerializeField] private bool t_isOnWall;
    [SerializeField] private int t_playerWallDirection;
    [SerializeField] private bool t_isWallJumping;
    [SerializeField] private bool t_isDetached;
    [SerializeField] private bool t_isHoldingMoveBtn;

    [Header("Movement")]
    [SerializeField] private float playerMaxVelocityX = 10f;
    [SerializeField] private float playerMaxVelocityY = 20f;
    [SerializeField] private float playerAccelerationX = 50f;
    [SerializeField] private float playerDecelerationX = 60f;
    [SerializeField] private float playerEarlyJumpAbortForceY = 0.5f;
    [SerializeField] private float playerMaxWallJumpVelocityX = 18f;
    [SerializeField] private float playerMaxWallJumpVelocityY = 12f;
    [SerializeField] private float wallSlideSpeed = -3f;

    [Header("Wall Jump Timer")]
    [SerializeField] private float wallJumpDuration = 0.15f;
    private float wallJumpTimer;
    
    [SerializeField] private float wallJumpCooldown = 0.2f;
    private float wallJumpCooldownTimer;
    
    [SerializeField] private float wallJumpAirControlDelay = 0.1f;
    private float wallJumpAirControlTimer;

    [Header("Coyote Time")]
    [SerializeField] private float groundCoyoteTime = 0.1f;
    [SerializeField] private float wallCoyoteTime = 0.15f;
    private float groundCoyoteCounter;
    private float wallCoyoteCounter;

    [Header("Multi Jump")]
    [SerializeField] private int extraJumps = 1;
    private int jumpCounter;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Collider Settings")]
    [SerializeField] private float wallCheckDistance = 0.2f;   
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.2f, 0.8f);
    [SerializeField] private BoxCollider2D boxCollider;

    private Rigidbody2D body;

    private float _horizontalInput;

    private bool _isGrounded;
    private bool _isOnWall;
    private bool _isWallJumping;
    private bool _isDetached;
    private bool _isWallSliding;
    private int _playerWallDirection;
    private int _lastWallDirection;
    private bool canDoubleJump = false;

    // SYSTEMVARIABLEN
    private bool showcaseDoubleJump = false;
    private bool inputLocked = false;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        
        // Versuche Collider zu finden
        if (boxCollider == null)
        {
            // 1. Erst am eigenen GameObject suchen
            boxCollider = GetComponent<BoxCollider2D>();
            
            // 2. Falls nicht gefunden, in Children suchen
            if (boxCollider == null)
            {
                boxCollider = GetComponentInChildren<BoxCollider2D>();
                
                if (boxCollider != null)
                {
                    Debug.Log($"BoxCollider2D gefunden in Child: {boxCollider.gameObject.name}");
                }
            }
        }
        
        if (boxCollider == null)
        {
            Debug.LogError("BoxCollider2D nicht gefunden!");
        }
    }

    private void Start()
    {
        if (SoulManager.Instance != null && SoulManager.Instance.HasSoul("rabbitSoul"))
            canDoubleJump = true;
    }

    private void Update()
    {
        if (inputLocked)
        {
            SetInputLocked(true);
            return;
        }

        t_isHoldingMoveBtn = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);

        // SPRUNG-LOGIK
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 1. Boden-Sprung
            if (_isGrounded || groundCoyoteCounter > 0)
            {
                Jump();
                jumpCounter = canDoubleJump ? extraJumps : 0;
            }
            // 2. Wall-Jump
            else if (CanWallJump())
            {
                WallJump();
            }
            // 3. Double-Jump
            else if (canDoubleJump && jumpCounter > 0)
            {
                Jump();
                jumpCounter--;
            }
        }

        // FRÜHER SPRUNG-ABBRUCH
        if (Input.GetKeyUp(KeyCode.Space) && body.linearVelocity.y > 0)
            body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y * playerEarlyJumpAbortForceY);

        // DOUBLE-JUMP DEMO
        if (showcaseDoubleJump && _isGrounded)
        {
            showcaseDoubleJump = false;
            StartCoroutine(PlayDoubleJumpShowcase());
        }
    }

    private bool CanWallJump()
    {
        return !_isWallJumping && (_isOnWall || wallCoyoteCounter > 0) && wallJumpCooldownTimer <= 0;
    }

    private void FixedUpdate()
    {
        if (inputLocked)
        {
            SetInputLocked(true);
            return;
        }

        // TIMER VERRINGERN
        if (wallJumpCooldownTimer > 0)
            wallJumpCooldownTimer -= Time.fixedDeltaTime;
        
        if (wallJumpAirControlTimer > 0)
            wallJumpAirControlTimer -= Time.fixedDeltaTime;

        // ZUSTANDS-ÜBERPRÜFUNG
        _isGrounded = t_isGrounded = CheckIsGrounded();
        
        bool wasOnWall = _isOnWall;
        
        if (wallJumpCooldownTimer <= 0)
        {
            _isOnWall = t_isOnWall = CheckOnWall();
            
            if (_isOnWall)
                _lastWallDirection = _playerWallDirection;
        }
        else
        {
            _isOnWall = t_isOnWall = false;
        }
        
        t_movementY = body.linearVelocity.y;

        float horizontalInput = _horizontalInput = t_movementDirection = Input.GetAxisRaw("Horizontal");
        
        // WALL-JUMP LOGIK (Priorität 1)
        if (_isWallJumping)
        {
            wallJumpTimer -= Time.fixedDeltaTime;
            
            if (wallJumpAirControlTimer <= 0)
            {
                float targetSpeed = horizontalInput * playerMaxVelocityX;
                float accelRate = playerAccelerationX * 0.5f;
                float speedDiff = targetSpeed - body.linearVelocity.x;
                float movement = speedDiff * accelRate * Time.fixedDeltaTime;
                
                body.linearVelocity = new Vector2(
                    Mathf.Clamp(body.linearVelocity.x + movement, -playerMaxVelocityX, playerMaxVelocityX),
                    body.linearVelocity.y
                );
            }
            
            if (wallJumpTimer <= 0 || _isGrounded)
            {
                _isWallJumping = t_isWallJumping = false;
                _isDetached = t_isDetached = false;
            }
        }

        // WANDGLEITEN (Priorität 2)
        else if (_isOnWall && !_isGrounded && Mathf.Abs(horizontalInput) > 0.1f)
        {
            if (Mathf.Sign(horizontalInput) == _playerWallDirection) 
            {
                _isWallSliding = true;

                if (body.linearVelocity.y < wallSlideSpeed)
                    body.linearVelocity = new Vector2(body.linearVelocity.x, wallSlideSpeed);

                wallCoyoteCounter = wallCoyoteTime;
                t_wallCoyoteCounter = wallCoyoteCounter;
                
                body.linearVelocity = new Vector2(0, body.linearVelocity.y);
            }
            else
            {
                _isWallSliding = false;
                t_isOnWall = false;
                
                if (wallCoyoteCounter <= 0)
                {
                    wallCoyoteCounter = wallCoyoteTime;
                    t_wallCoyoteCounter = wallCoyoteCounter;
                }
                
                ApplyNormalMovement(horizontalInput);
            }
        }
        
        // NORMALE BEWEGUNG (Priorität 3)
        else
        {
            if (!_isOnWall && wallCoyoteCounter > 0)
            {
                wallCoyoteCounter -= Time.fixedDeltaTime;
                t_wallCoyoteCounter = wallCoyoteCounter;
            }
            
            _isWallSliding = false;
            t_isOnWall = false;
            ApplyNormalMovement(horizontalInput);
        }

        // BODEN-HANDLING
        if (_isGrounded)
        {
            _isWallJumping = t_isWallJumping = false;
            _isDetached = t_isDetached = false;
            _isWallSliding = false;
            
            groundCoyoteCounter = t_groundCoyoteCounter = groundCoyoteTime;
            jumpCounter = canDoubleJump ? extraJumps : 0;
            wallCoyoteCounter = t_wallCoyoteCounter = 0;
            
            wallJumpCooldownTimer = 0;
            wallJumpAirControlTimer = 0;
        }
        else if (groundCoyoteCounter > 0)
        {
            groundCoyoteCounter -= Time.fixedDeltaTime;
            t_groundCoyoteCounter = groundCoyoteCounter;
        }

        // SPIELER-SPRITE RICHTEN
        if (horizontalInput != 0)
        {
            float dir = horizontalInput > 0 ? 1 : -1;
            transform.localScale = new Vector3(dir * 0.7f, 0.7f, 1);
        }
    }

    private void ApplyNormalMovement(float horizontalInput)
    {
        float targetSpeed = horizontalInput * playerMaxVelocityX;
        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? playerAccelerationX : playerDecelerationX;
        float speedDiff = targetSpeed - body.linearVelocity.x;
        float movement = speedDiff * accelRate * Time.fixedDeltaTime;
        
        body.linearVelocity = new Vector2(
            Mathf.Clamp(body.linearVelocity.x + movement, -playerMaxVelocityX, playerMaxVelocityX),
            body.linearVelocity.y
        );
    }

    // SPRUNGFUNKTIONEN

    private void Jump()
    {
        body.linearVelocity = new Vector2(body.linearVelocity.x, playerMaxVelocityY);

        if (groundCoyoteCounter > 0)
            groundCoyoteCounter = t_groundCoyoteCounter = 0;
        else if (jumpCounter > 0)
        {
            jumpCounter--;
            _isWallJumping = false;
            t_isDetached = _isDetached = false;
        }
    }

    private void WallJump()
    {
        int wallDir = _isOnWall ? _playerWallDirection : _lastWallDirection;
        float jumpDirX = -wallDir;

        Vector2 jumpForce = new Vector2(
            jumpDirX * playerMaxWallJumpVelocityX, 
            playerMaxWallJumpVelocityY
        );
        body.linearVelocity = jumpForce;

        _isWallJumping = t_isWallJumping = true;
        wallJumpTimer = wallJumpDuration;
        wallJumpAirControlTimer = wallJumpAirControlDelay;
        wallJumpCooldownTimer = wallJumpCooldown;
        
        _isOnWall = false;
        _isWallSliding = false;
        wallCoyoteCounter = t_wallCoyoteCounter = 0;
        _isDetached = t_isDetached = true;
        jumpCounter = canDoubleJump ? extraJumps : 0;
    }

    // KOLLISIONSERKENNUNG 

    private bool CheckIsGrounded()
    {
        if (boxCollider == null) return false;

        // Bei 45° rotiertem Collider: Die unterste Spitze des Diamanten finden
        Vector2 bottomPoint = new Vector2(
            boxCollider.bounds.center.x,
            boxCollider.bounds.min.y  // Unterster Punkt des Bounds
        );

        // Mehrere Raycasts für bessere Erkennung
        float raySpacing = 0.15f;  // Abstand zwischen den Rays
        bool isGrounded = false;

        // 3 Raycasts: Mitte, Links, Rechts
        for (int i = -1; i <= 1; i++)
        {
            Vector2 rayOrigin = bottomPoint + Vector2.right * (i * raySpacing);
            
            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin,
                Vector2.down,
                groundCheckDistance,
                groundLayer
            );

            if (hit.collider != null)
            {
                isGrounded = true;
            }
        }

        return isGrounded;
    }

    private bool CheckOnWall()
    {
        if (boxCollider == null || _isGrounded || wallJumpCooldownTimer > 0) 
            return false;

        float direction = Mathf.Sign(_horizontalInput);

        if (Mathf.Abs(direction) < 0.1f)
            return false;

        // Prüfposition an der Seite des rotierten Colliders
        Vector2 checkPos = boxCollider.bounds.center;
        float checkDistance = boxCollider.bounds.extents.x + wallCheckDistance;
        checkPos.x += checkDistance * direction;

        // BoxCast für Wand-Erkennung
        Vector2 castSize = new Vector2(wallCheckSize.x, boxCollider.bounds.size.y * 0.8f);
        
        RaycastHit2D hit = Physics2D.BoxCast(
            checkPos, castSize, 0f, Vector2.zero, 0f, wallLayer);

        // Fallback: Multiple Raycasts für bessere Erkennung
        if (!hit.collider)
        {
            Vector2 top = checkPos + Vector2.up * (boxCollider.bounds.extents.y * 0.6f);
            Vector2 bottom = checkPos + Vector2.down * (boxCollider.bounds.extents.y * 0.6f);

            RaycastHit2D hitTop = Physics2D.Raycast(top, Vector2.right * direction, wallCheckDistance, wallLayer);
            RaycastHit2D hitMiddle = Physics2D.Raycast(checkPos, Vector2.right * direction, wallCheckDistance, wallLayer);
            RaycastHit2D hitBottom = Physics2D.Raycast(bottom, Vector2.right * direction, wallCheckDistance, wallLayer);

            hit = hitTop.collider != null ? hitTop : hitMiddle.collider != null ? hitMiddle : hitBottom;
        }

        if (hit.collider)
        {
            _playerWallDirection = t_playerWallDirection = (hit.normal.x > 0) ? -1 : 1;
            return true;
        }

        return false;
    }

    public bool IsGrounded() => _isGrounded;
    public bool IsOnWall() => _isOnWall;
    public bool IsWallSliding() => _isWallSliding;
    public bool CanAttack() => _isGrounded;


    public void OnSoulCollected(SoulData soul)
    {
        if (soul == null) return;
        
        if (soul.soulID == "rabbitSoul")
        {
            canDoubleJump = true;
            showcaseDoubleJump = true;
        }
    }

    private IEnumerator PlayDoubleJumpShowcase()
    {
        inputLocked = true;
        
        while (!_isGrounded) yield return null;

        yield return new WaitForSeconds(0.3f);
        Jump();
        yield return new WaitForSeconds(0.3f);

        jumpCounter = 1;
        Jump();

        yield return new WaitForSeconds(2f);
        inputLocked = false;
    }

    public void SetInputLocked(bool locked)
    {
        inputLocked = locked;
        if (locked)
        {
            ResetHorizontalInputAndVelocity();
        }
    }

    public bool IsInputLocked() => inputLocked;

    public void ResetHorizontalInputAndVelocity()
    {
        _horizontalInput = 0;
        body.linearVelocity = new Vector2(0, body.linearVelocity.y);
    }
}