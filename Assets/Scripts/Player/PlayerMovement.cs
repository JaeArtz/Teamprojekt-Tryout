using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Tracker")]
    [SerializeField] private float t_movementX;
    [SerializeField] private float t_movementY;
    [SerializeField] private float t_movementDirection;
    [SerializeField] private float t_coyoteCounter;
    [SerializeField] private bool t_isGrounded;
    [SerializeField] private bool t_isOnWall;
    [SerializeField] private bool t_isWallJumping;
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
    [SerializeField] private float coyoteTime; //Time player can still jump after leaving ground
    private float coyoteCounter; //how much time since leaving ground or object

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

    private bool canDoubleJump = false; //Für Hasenseele


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
        // Flip Player Sprite when moving left/right
        //float horizontalInput = Input.GetAxis("Horizontal");
        //if (horizontalInput > 0.01)
        //{
        //    transform.localScale = Vector3.one * 0.7f;
        //}
        //else if (horizontalInput < -0.01)
        //{
        //    transform.localScale = new Vector3(-1, 1, 1) * 0.7f;
        //}


        // Wegen Hasenseele, sollte alre funktionalität aber nicht verändern!
        t_isHoldingMoveBtn = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(_isGrounded || coyoteCounter > 0)
            {
                Jump();
                jumpCounter = canDoubleJump ? extraJumps : 0; //Reset extra jumps after normal jump
            }
            else if(canDoubleJump && jumpCounter > 0)
            {
                Jump();
                jumpCounter = 0; // Use one jump for double jump
            }
            else if (_isOnWall && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
            {
                WallJump();
                _isWallJumping = t_isWallJumping = true;
                _isOnWall = false;
                jumpCounter = canDoubleJump ? extraJumps : 0; //Reset extra jumps after walljump
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

        DrawBoxCast(transform.position, new Vector2(1f, 2f), 0f, Vector2.down, 0.1f, Color.red);
    }

    private void FixedUpdate()
    {
        _isGrounded = t_isGrounded = isGrounded();
        _isOnWall = t_isOnWall = onWall();

        t_movementY = body.linearVelocityY;

        float horizontalInput = _horizontalInput = t_movementDirection = Input.GetAxisRaw("Horizontal");

        if (_isWallJumping && !_isOnWall && !_isDetached)
        {
            // Spieler hat Wand verlassen nach Walljump
            _isDetached = true;
        }

        if (_isOnWall && _isDetached)
        {
            // Abbruch Walljump, wenn Wand ber�hrt wird
            _isWallJumping = t_isWallJumping = false;
            _isDetached = false;
            body.linearVelocityX = t_movementX = 0;
        }
        else if (_isWallJumping)
        {
            //logik f�r Bewegung a/d w�hrend walljump (ged�mpft)
            float difference = horizontalInput * playerMaxVelocityX - body.linearVelocityX;
            body.linearVelocityX = t_movementX = body.linearVelocityX + difference * Time.deltaTime * 1.5f;
        }
        else if (_isOnWall)
        {
            // Kein seitlicher Input an der Wand
            // body.linearVelocityX = t_movementX = 0;
        }
        else
        {
            // Normale Bewegung
            float targetSpeed = horizontalInput * playerMaxVelocityX;
            float speedDiff = targetSpeed - body.linearVelocityX;
            float accelRate = Mathf.Sign(body.linearVelocityX) == horizontalInput ? playerAccelerationX : playerDecelerationX;
            float movement = speedDiff * accelRate;
            body.linearVelocityX = t_movementX = Mathf.MoveTowards(body.linearVelocityX, targetSpeed, accelRate);
            //body.linearVelocityX = t_movementX = horizontalInput * playerMaxVelocityX;
        }

        if (_isGrounded)
        {
            // Stelle alle Sprungvariablen zur�ck w�hrend auf dem Boden
            _isWallJumping = t_isWallJumping = false;
            _isDetached = false;
            coyoteCounter = t_coyoteCounter = coyoteTime;
            jumpCounter = canDoubleJump ? extraJumps : 0;
        }
        else
        {
            // Wenn nicht auf dem Boden, verringere den Coyote Timer
            coyoteCounter -= Time.deltaTime; //Start decrease counter
            t_coyoteCounter = coyoteCounter;
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
    }

    private void Jump()
    {
        if (_isGrounded)
        {
            body.linearVelocityY = playerMaxVelocityY;
        }
        else if (coyoteCounter > 0)
        {
            body.linearVelocityY = playerMaxVelocityY;
            coyoteCounter = t_coyoteCounter = 0;
        }
        else if (jumpCounter > 0)
        {
            body.linearVelocityY = playerMaxVelocityY;
            jumpCounter--;
            _isWallJumping = false;
            _isDetached = false;
        }
    }

    private void WallJump()
    {
        Vector2 wallJumpDirection = new Vector2(-_horizontalInput * playerMaxWallJumpVelocityX, playerMaxWallJumpVelocityY); //Jump in opposite direction of wall
        body.linearVelocity = wallJumpDirection;
    }


    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, surfaceLayer);
        return raycastHit.collider;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit2 = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(Mathf.Sign(_horizontalInput), 0), 0.1f, surfaceLayer);
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

        //weiter einfach hinzufügen
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