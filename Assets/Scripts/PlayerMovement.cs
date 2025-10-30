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
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpDampingForce;
    [SerializeField] private float wallJumpForce;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime; //Time player can still jump after leaving ground
    private float coyoteCounter; //how much time since leaving ground or object

    [Header("Multi Jump")]
    [SerializeField] private int extraJumps;
    private int jumpCounter;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D body;
    private BoxCollider2D boxCollider;

    private float _horizontalInput;

    private bool _isGrounded;
    private bool _isOnWall;
    private bool _isWallJumping;
    private bool _isDetached;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        t_isHoldingMoveBtn = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
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
        }

        if (Input.GetKeyUp(KeyCode.Space) && body.linearVelocityY > 0)
        {
            body.linearVelocityY *= jumpDampingForce;
        }

    }

    private void FixedUpdate()
    {
        _isGrounded = t_isGrounded = isGrounded();
        _isOnWall = t_isOnWall = onWall();

        t_movementY = body.linearVelocityY;

        float horizontalInput = _horizontalInput = t_movementDirection = Input.GetAxis("Horizontal");

        if (_isWallJumping && !_isOnWall && !_isDetached)
        {
            // Spieler hat Wand verlassen nach Walljump
            _isDetached = true;
        }

        if (_isOnWall && _isDetached)
        {
            // Abbruch Walljump, wenn Wand berührt wird
            _isWallJumping = t_isWallJumping = false;
            _isDetached = false;
            body.linearVelocityX = t_movementX = 0;
        }
        else if (_isWallJumping)
        {
            //logik für Bewegung a/d während walljump (gedämpft)
            float difference = horizontalInput * speed - body.linearVelocityX;
            body.linearVelocityX = t_movementX = body.linearVelocityX + difference * Time.deltaTime * 1.5f;
        }
        else if (_isOnWall)
        {
            // Kein seitlicher Input an der Wand
            body.linearVelocityX = t_movementX = 0;
        }
        else
        {
            // Normale Bewegung
            body.linearVelocityX = t_movementX = horizontalInput * speed;
        }

        if (_isGrounded)
        {
            // Stelle alle Sprungvariablen zurück während auf dem Boden
            _isWallJumping = t_isWallJumping = false;
            _isDetached = false;
            coyoteCounter = t_coyoteCounter = coyoteTime;
            jumpCounter = extraJumps;
        }
        else
        {
            // Wenn nicht auf dem Boden, verringere den Coyote Timer
            coyoteCounter -= Time.deltaTime; //Start decrease counter
            t_coyoteCounter = coyoteCounter;
        }
    }

    private void Jump()
    {
        if (coyoteCounter <= 0 && jumpCounter <= 0)
        {
            return; //No jump if coyote time is over
        }

        if (_isGrounded)
        {
            body.linearVelocityY = jumpForce;
        }
        else
        {
            if (coyoteCounter > 0)
            {
                body.linearVelocityY = jumpForce;
            }
            else
            {
                if (jumpCounter > 0)
                {
                    body.linearVelocityY = jumpForce;
                    jumpCounter--;
                    _isWallJumping = false;
                    _isDetached = false;
                }
            }
        }

        coyoteCounter = t_coyoteCounter = 0; //Reset coyote counter after jump

    }

    private void WallJump()
    {
        Vector2 wallJumpDirection = new Vector2(-_horizontalInput * speed * 2, wallJumpForce); //Jump in opposite direction of wall
        body.linearVelocity = wallJumpDirection;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {

        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(Mathf.Sign(_horizontalInput), 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }
}