using System;
using TMPro;
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    // ===== TRACKER FÜR DEBUG/INSPECTOR ANZEIGE =====
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

    // ===== BEWEGUNGSPARAMETER =====
    [Header("Movement")]
    [SerializeField] private float playerMaxVelocityX = 10f;
    [SerializeField] private float playerMaxVelocityY = 20f;
    [SerializeField] private float playerAccelerationX = 50f;
    [SerializeField] private float playerDecelerationX = 60f;
    [SerializeField] private float playerEarlyJumpAbortForceY = 0.5f;
    [SerializeField] private float playerMaxWallJumpVelocityX = 12f;
    [SerializeField] private float playerMaxWallJumpVelocityY = 18f;
    [SerializeField] private float wallSlideSpeed = -3f;

    // ===== WALL-JUMP TIMER =====
    [Header("Wall Jump Timer")]
    [SerializeField] private float wallJumpDuration = 0.15f;    // Verringert für mehr Kontrolle
    private float wallJumpTimer;
    
    // Cooldown nach Wall-Jump
    [SerializeField] private float wallJumpCooldown = 0.2f;
    private float wallJumpCooldownTimer;
    
    // Erlaubt Luftsteuerung während Wall-Jump
    [SerializeField] private float wallJumpAirControlDelay = 0.1f;  // Nach dieser Zeit hat man wieder volle Kontrolle
    private float wallJumpAirControlTimer;

    // ===== COYOTE TIME =====
    [Header("Coyote Time")]
    [SerializeField] private float groundCoyoteTime = 0.1f;
    [SerializeField] private float wallCoyoteTime = 0.15f;    // Etwas erhöht für besseres Gefühl
    private float groundCoyoteCounter;
    private float wallCoyoteCounter;

    // ===== MEHRFACHSPRUNG =====
    [Header("Multi Jump")]
    [SerializeField] private int extraJumps = 1;
    private int jumpCounter;

    // ===== PHYSICS LAYER =====
    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    // ===== COLLIDER EINSTELLUNGEN =====
    [Header("Collider Settings")]
    [SerializeField] private float wallCheckDistance = 0.1f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.2f, 1f);

    // ===== REFERENZEN =====
    private Rigidbody2D body;
    private PolygonCollider2D polyCollider;
    private Vector2[] colliderPoints;

    // ===== EINGABE =====
    private float _horizontalInput;

    // ===== SPIELERZUSTÄNDE =====
    private bool _isGrounded;
    private bool _isOnWall;
    private bool _isWallJumping;
    private bool _isDetached;
    private bool _isWallSliding;
    private int _playerWallDirection;
    private int _lastWallDirection;  // Merkt sich die letzte Wand-Richtung
    private bool canDoubleJump = false;

    // ===== SYSTEMVARIABLEN =====
    private bool showcaseDoubleJump = false;
    private bool inputLocked = false;

    // ===== INITIALISIERUNG =====
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        polyCollider = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        if (polyCollider != null)
            colliderPoints = polyCollider.points;

        if (SoulManager.Instance != null && SoulManager.Instance.HasSoul("rabbitSoul"))
            canDoubleJump = true;
    }

    // ===== UPDATE (EINGABE-HANDLING) =====
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
            // 1. Boden-Sprung (inkl. Coyote-Time)
            if (_isGrounded || groundCoyoteCounter > 0)
            {
                Jump();
                jumpCounter = canDoubleJump ? extraJumps : 0;
            }
            // 2. Wall-Jump (NUR wenn Cooldown abgelaufen und an Wand/Coyote-Time)
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

    // Prüft, ob ein Wall-Jump möglich ist
    private bool CanWallJump()
    {
        return !_isWallJumping && (_isOnWall || wallCoyoteCounter > 0) && wallJumpCooldownTimer <= 0;
    }

    // ===== FIXEDUPDATE (PHYSIK-HANDLING) =====
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
        
        // Wand-Check (IMMER durchführen, nicht nur im Cooldown!)
        bool wasOnWall = _isOnWall;
        
        if (wallJumpCooldownTimer <= 0)
        {
            _isOnWall = t_isOnWall = CheckOnWall();
            
            // Letzte Wand-Richtung merken, wenn wir an einer Wand sind
            if (_isOnWall)
                _lastWallDirection = _playerWallDirection;
        }
        else
        {
            _isOnWall = t_isOnWall = false;
        }
        
        t_movementY = body.linearVelocity.y;

        // EINGABE LESEN
        float horizontalInput = _horizontalInput = t_movementDirection = Input.GetAxisRaw("Horizontal");

        // === BEWEGUNGS-ZUSTANDSMASCHINE ===
        
        // WALL-JUMP LOGIK (Priorität 1)
        if (_isWallJumping)
        {
            wallJumpTimer -= Time.fixedDeltaTime;
            
            // Nach kurzer Zeit erlauben wir Luftsteuerung für smooth Kontrolle
            if (wallJumpAirControlTimer <= 0)
            {
                // Spieler hat wieder teilweise Kontrolle (50% der normalen Beschleunigung)
                float targetSpeed = horizontalInput * playerMaxVelocityX;
                float accelRate = playerAccelerationX * 0.5f; // Reduzierte Kontrolle während Wall-Jump
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
            // NUR an Wand gleiten, wenn AKTIV in Richtung der Wand gedrückt wird
            if (Mathf.Sign(horizontalInput) == _playerWallDirection) 
            {
                _isWallSliding = true;

                // Vertikale Geschwindigkeit begrenzen
                if (body.linearVelocity.y < wallSlideSpeed)
                    body.linearVelocity = new Vector2(body.linearVelocity.x, wallSlideSpeed);

                // Wand-Coyote-Time zurücksetzen
                wallCoyoteCounter = wallCoyoteTime;
                t_wallCoyoteCounter = wallCoyoteCounter;
                
                // Horizontale Geschwindigkeit auf 0 (an Wand kleben)
                body.linearVelocity = new Vector2(0, body.linearVelocity.y);
            }
            else
            {
                // Spieler drückt weg von der Wand -> normale Bewegung
                _isWallSliding = false;
                t_isOnWall = false;
                
                // Wand-Coyote-Time starten wenn wir loslassen
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
            // Wand-Coyote-Time verringern
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
            
            // Cooldown zurücksetzen
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

    // Hilfsmethode für normale Bewegung
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

    // ===== SPRUNGFUNKTIONEN =====

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
        // Sprungrichtung bestimmen
        // Wenn wir an einer Wand sind, nutzen wir deren Richtung
        // Wenn wir in Coyote-Time sind, nutzen wir die letzte Wand-Richtung
        int wallDir = _isOnWall ? _playerWallDirection : _lastWallDirection;
        
        // Sprung geht IMMER von der Wand weg
        float jumpDirX = -wallDir;

        // Horizontaler Impuls etwas stärker für besseres Gefühl
        Vector2 jumpForce = new Vector2(
            jumpDirX * playerMaxWallJumpVelocityX, 
            playerMaxWallJumpVelocityY
        );
        body.linearVelocity = jumpForce;

        // Zustände setzen
        _isWallJumping = t_isWallJumping = true;
        wallJumpTimer = wallJumpDuration;
        
        // Timer für Luftsteuerung starten
        wallJumpAirControlTimer = wallJumpAirControlDelay;
        
        // Cooldown starten (verhindert Wall-Climbing)
        wallJumpCooldownTimer = wallJumpCooldown;
        
        _isOnWall = false;
        _isWallSliding = false;
        wallCoyoteCounter = t_wallCoyoteCounter = 0;
        _isDetached = t_isDetached = true;
        jumpCounter = canDoubleJump ? extraJumps : 0;
    }

    // ===== KOLLISIONSERKENNUNG =====

    private bool CheckIsGrounded()
    {
        if (polyCollider == null || colliderPoints == null) return false;

        float minY = float.MaxValue;
        foreach (Vector2 point in colliderPoints)
        {
            Vector2 worldPoint = transform.TransformPoint(point);
            if (worldPoint.y < minY) minY = worldPoint.y;
        }

        Vector2 castOrigin = new Vector2(transform.position.x, minY + 0.01f);
        float castWidth = polyCollider.bounds.size.x * 0.8f;

        RaycastHit2D hit = Physics2D.BoxCast(
            castOrigin, 
            new Vector2(castWidth, 0.05f),
            0f, 
            Vector2.down, 
            groundCheckDistance, 
            groundLayer
        );

        Debug.DrawRay(castOrigin, Vector2.down * groundCheckDistance, hit.collider ? Color.green : Color.red);

        return hit.collider != null;
    }

    private bool CheckOnWall()
    {
        // Keine Wand-Erkennung wenn am Boden oder im Cooldown
        if (polyCollider == null || _isGrounded || wallJumpCooldownTimer > 0) 
            return false;

        float direction = Mathf.Sign(_horizontalInput);

        // Ohne Input KEINE Wand-Erkennung - sofort raus!
        if (Mathf.Abs(direction) < 0.1f)
            return false;

        Vector2 checkPos = polyCollider.bounds.center;
        checkPos.x += (polyCollider.bounds.extents.x + wallCheckDistance) * direction;

        // BoxCast für Wand-Erkennung
        RaycastHit2D hit = Physics2D.BoxCast(
            checkPos, 
            new Vector2(wallCheckSize.x, polyCollider.bounds.size.y * 0.9f),
            0f, 
            Vector2.zero, 
            0f, 
            wallLayer
        );

        // Fallback: Raycasts
        if (!hit.collider)
        {
            Vector2 top = checkPos + Vector2.up * (polyCollider.bounds.extents.y * 0.7f);
            Vector2 bottom = checkPos + Vector2.down * (polyCollider.bounds.extents.y * 0.7f);

            RaycastHit2D hitTop = Physics2D.Raycast(top, Vector2.right * direction, 0.05f, wallLayer);
            RaycastHit2D hitMiddle = Physics2D.Raycast(checkPos, Vector2.right * direction, 0.05f, wallLayer);
            RaycastHit2D hitBottom = Physics2D.Raycast(bottom, Vector2.right * direction, 0.05f, wallLayer);

            hit = hitTop.collider != null ? hitTop : hitMiddle.collider != null ? hitMiddle : hitBottom;
        }

        if (hit.collider)
        {
            _playerWallDirection = t_playerWallDirection = (hit.normal.x > 0) ? -1 : 1;
            return true;
        }

        return false;
    }

    // ===== ÖFFENTLICHE METHODEN =====

    public bool IsGrounded() => _isGrounded;
    public bool IsOnWall() => _isOnWall;
    public bool IsWallSliding() => _isWallSliding;
    public bool CanAttack() => _isGrounded;
    public bool CanAttack(bool allowWhileWallSliding) => 
        allowWhileWallSliding ? _isGrounded || (_isOnWall && body.linearVelocity.y < 0) : _isGrounded;

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
            _horizontalInput = 0;
            body.linearVelocity = new Vector2(0, body.linearVelocity.y);
        }
    }

    public bool IsInputLocked() => inputLocked;

    public void ResetHorizontalInputAndVelocity()
    {
        _horizontalInput = 0;
        body.linearVelocity = new Vector2(0, body.linearVelocity.y);
    }
}