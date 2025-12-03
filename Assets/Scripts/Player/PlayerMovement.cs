using System;
using TMPro;
using UnityEngine;

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
    [SerializeField] private float playerMaxVelocityX;
    [SerializeField] private float playerMaxVelocityY;
    [SerializeField] private float playerAccelerationX;
    [SerializeField] private float playerDecelerationX;
    [SerializeField] private float playerEarlyJumpAbortForceY;
    [SerializeField] private float playerMaxWallJumpVelocityX;
    [SerializeField] private float playerMaxWallJumpVelocityY;

    [Header("Coyote Time")]
    [SerializeField] private float groundCoyoteTime; //Time player can still jump after leaving ground
    [SerializeField] private float wallCoyoteTime; //Time player can still jump after leaving wall
    private float groundCoyoteCounter; //how much time since leaving ground or object
    private float wallCoyoteCounter; //how much time since leaving ground or object

    [Header("Multi Jump")]
    [SerializeField] private int extraJumps;
    private int jumpCounter;

    [Header("Layers")]
    [SerializeField] private LayerMask surfaceLayer;

    private Rigidbody2D body;
    private BoxCollider2D boxCollider;

    private float _horizontalInput;

    private bool _isGrounded;
    private bool _isOnWall;
    private bool _isWallJumping;
    private bool _isDetached;
    private bool _isWallSliding;
    private int _playerWallDirection;
    private bool canDoubleJump = false; // For Bunny-Soul


    private void Start()
    {
        if(SoulManager.Instance != null && SoulManager.Instance.HasSoul("rabbitSoul"))
        {
            canDoubleJump = true;
        }
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // Because Bunny-Soul, shouldn't affect other functionalities!
        t_isHoldingMoveBtn = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(_isGrounded || groundCoyoteCounter > 0)
            {
                Jump();
                jumpCounter = canDoubleJump ? extraJumps : 0; //Reset extra jumps after normal jump
            }
            else if (canWallJump())
            {
                WallJump();
                _isWallJumping = t_isWallJumping = true;
                _isOnWall = false;
                jumpCounter = canDoubleJump ? extraJumps : 0; //Reset extra jumps after walljump
            }
            else if(canDoubleJump && jumpCounter > 0)
            {
                Jump();
                jumpCounter = 0; // Use one jump for double jump
            }

        }

        /*
        if (Input.GetKeyDown(KeyCode.Space) && (coyoteCounter > 0 || jumpCounter > 0) && (!_isOnWall || _isGrounded))
        {
            Jump();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && _isOnWall && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            WallJump();
            _isWallJumping = t_isWallJumping = true;
            _isOnWall = false;
            jumpCounter = extraJumps; //Reset extra jumps after walljump
        }*/

        if (Input.GetKeyUp(KeyCode.Space) && body.linearVelocityY > 0)
        {
            body.linearVelocityY *= playerEarlyJumpAbortForceY;
        }

        
    }

    private bool canWallJump()
    {
        return !_isWallJumping && (_isOnWall || wallCoyoteCounter > 0) && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D));
    }

    /// <summary>
    /// ACP: Attention! -> Vector3 is also being used as a reference for RespawnPoints
    /// </summary>
    private void FixedUpdate()
    {
        _isGrounded = t_isGrounded = isGrounded();
        _isOnWall = t_isOnWall = onWall();

        t_movementY = body.linearVelocityY;

        float horizontalInput = _horizontalInput = t_movementDirection = Input.GetAxisRaw("Horizontal");

        if (_isWallJumping && !_isOnWall && !_isDetached)
        {
            // Player has left wall after walljump
            t_isDetached = _isDetached = true;
            _isWallSliding = false;
        }

        if (_isOnWall && _isDetached)
        {
            // Disruption of walljump when touching wall again
            _isWallJumping = t_isWallJumping = false;
            t_isDetached = _isDetached = false;
            body.linearVelocityX = t_movementX = 0;
            wallCoyoteCounter = wallCoyoteTime;
            t_wallCoyoteCounter = wallCoyoteCounter;
        }
        else if (_isWallJumping)
        {
            //logic for movement a/d while walljumping (dampened)
            float difference = horizontalInput * playerMaxVelocityX - body.linearVelocityX;
            body.linearVelocityX = t_movementX = body.linearVelocityX + difference * Time.deltaTime * 1.5f;
            if (wallCoyoteCounter > 0)
            {
                wallCoyoteCounter -= Time.deltaTime;
                t_wallCoyoteCounter = wallCoyoteCounter;
            }
        }
        else if (_isOnWall)
        {
            // no velocityX while touching wall
            // body.linearVelocityX = t_movementX = 0; // Not needed for now, add later when body movements on wall create unwanted effects
            wallCoyoteCounter = wallCoyoteTime;
            t_wallCoyoteCounter = wallCoyoteCounter;
            if (body.linearVelocityY < 0)
                _isWallSliding = true;
        }
        else if (_isWallSliding)
        {
            float targetSpeed = horizontalInput * playerMaxVelocityX * 0.2f;
            float speedDiff = targetSpeed - body.linearVelocityX;
            float accelRate = Mathf.Sign(body.linearVelocityX) == horizontalInput ? playerAccelerationX : playerDecelerationX;
            float movement = speedDiff * accelRate;
            body.linearVelocityX = t_movementX = Mathf.MoveTowards(body.linearVelocityX, targetSpeed, accelRate);

            if (!diminishWallCoyoteCounter())
                _isWallSliding = false;
        }
        else
        {
            // Normal movement
            float targetSpeed = horizontalInput * playerMaxVelocityX;
            float speedDiff = targetSpeed - body.linearVelocityX;
            float accelRate = Mathf.Sign(body.linearVelocityX) == horizontalInput ? playerAccelerationX : playerDecelerationX;
            float movement = speedDiff * accelRate;
            body.linearVelocityX = t_movementX = Mathf.MoveTowards(body.linearVelocityX, targetSpeed, accelRate);
            //body.linearVelocityX = t_movementX = horizontalInput * playerMaxVelocityX;
            diminishWallCoyoteCounter();
        }

        if (_isGrounded)
        {
            // All jump variables back to default while grounded
            _isWallJumping = t_isWallJumping = false;
            t_isDetached = _isDetached = false;
            _isWallSliding = false;
            groundCoyoteCounter = t_groundCoyoteCounter = groundCoyoteTime;
            jumpCounter = canDoubleJump ? extraJumps : 0;
        }
        else if (groundCoyoteCounter > 0)
        {
            // If not on ground, diminish Coyote Timer
            groundCoyoteCounter -= Time.deltaTime; //Start decrease counter
            t_groundCoyoteCounter = groundCoyoteCounter;
        }

        if (horizontalInput == 0)
        {
            //transform.localScale = new Vector3(-Mathf.Sign(horizontalInput), 1, 1) * 0.7f;
        }
        else if(Mathf.Sign(body.linearVelocityX) != horizontalInput)
        {
            // Check in which state the player is in (wall jumping, walking, etc.)
            // if walking, Break Animation in opposite direction of Mathf.Sign(horizontalInput)
            // placeholder:
            transform.localScale = new Vector3(-Mathf.Sign(horizontalInput), 1, 1) * 0.7f;
        }
        else
        {
            transform.localScale = new Vector3(horizontalInput, 1, 1) * 0.7f;
        }

        // DrawBoxCast(transform.position, new Vector2(1f, 2f), 0f, Vector2.down, 0.1f, Color.red);
    }

    private bool diminishWallCoyoteCounter()
    {
        if (wallCoyoteCounter <= 0)
        {
            return false;
        }

        wallCoyoteCounter -= Time.deltaTime;
        t_wallCoyoteCounter = wallCoyoteCounter;

        return true;
    }

    private void Jump()
    {
        if (_isGrounded)
        {
            body.linearVelocityY = playerMaxVelocityY;
        }
        else if (groundCoyoteCounter > 0)
        {
            body.linearVelocityY = playerMaxVelocityY;
            groundCoyoteCounter = t_groundCoyoteCounter = 0;
        }
        else if (jumpCounter > 0)
        {
            body.linearVelocityY = playerMaxVelocityY;
            jumpCounter--;
            _isWallJumping = false;
            t_isDetached = _isDetached = false;
        }
    }

    private void WallJump()
    {
        Vector2 wallJumpDirection = new Vector2(-_playerWallDirection * playerMaxWallJumpVelocityX, playerMaxWallJumpVelocityY); //Jump in opposite direction of wall
        body.linearVelocity = wallJumpDirection;
    }


    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, surfaceLayer);
        return raycastHit.collider;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit2 = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(Mathf.Sign(_horizontalInput), 0), 0.2f, surfaceLayer);
        if (raycastHit2.collider)
            t_playerWallDirection = _playerWallDirection = -Math.Sign(raycastHit2.normal.x);
        return raycastHit2.collider;
    }

    public bool canAttack()
    {
        return isGrounded(); //could add more, like !onWall()
    }

    public void OnSoulCollected(SoulData soul)
    {
        if(soul == null) return;
        if (soul.soulID == "rabbitSoul")
        {
            canDoubleJump = true;
        }

        //just add more here if present
    }

    void DrawBoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, Color color)
    {
        // Berechne die vier Ecken der Box am Startpunkt
        Vector2 halfSize = size * 0.5f;
        Quaternion rot = Quaternion.Euler(0, 0, angle);
        Vector2 right = rot * Vector2.right * halfSize.x;
        Vector2 up = rot * Vector2.up * halfSize.y;

        Vector2 topLeft = origin - right + up;
        Vector2 topRight = origin + right + up;
        Vector2 bottomLeft = origin - right - up;
        Vector2 bottomRight = origin + right - up;

        // Berechne die Endposition
        Vector2 move = direction.normalized * distance;
        Vector2 topLeftEnd = topLeft + move;
        Vector2 topRightEnd = topRight + move;
        Vector2 bottomLeftEnd = bottomLeft + move;
        Vector2 bottomRightEnd = bottomRight + move;

        // Zeichne Startbox
        Debug.DrawLine(topLeft, topRight, color);
        Debug.DrawLine(topRight, bottomRight, color);
        Debug.DrawLine(bottomRight, bottomLeft, color);
        Debug.DrawLine(bottomLeft, topLeft, color);

        // Zeichne Endbox
        Debug.DrawLine(topLeftEnd, topRightEnd, color);
        Debug.DrawLine(topRightEnd, bottomRightEnd, color);
        Debug.DrawLine(bottomRightEnd, bottomLeftEnd, color);
        Debug.DrawLine(bottomLeftEnd, topLeftEnd, color);

        // Verbinde Start- und Endpunkte
        Debug.DrawLine(topLeft, topLeftEnd, color);
        Debug.DrawLine(topRight, topRightEnd, color);
        Debug.DrawLine(bottomLeft, bottomLeftEnd, color);
        Debug.DrawLine(bottomRight, bottomRightEnd, color);
    }

}