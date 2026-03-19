using System;
using TMPro;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public enum PlayerState
{
    DEFAULT = 0,
    RUNNING = 1,
    JUMPING = 2,
    FALLING = 3,
    WALLSLIDING = 4,
    WALLJUMPING = 5,
    CLIMBING = 6,
}

public class PlayerController : MonoBehaviour
{
    [Header("Layers")]
    [SerializeField, Tooltip("The ground layer")] private LayerMask groundLayer;
    [SerializeField, Tooltip("The wall layer")] private LayerMask wallLayer;

    [Header("Collider Settings")]
    [SerializeField, Tooltip("How far the cast start is away from the player collider to check for a nearby wall")] private float wallCheckDistance = 0.2f;
    [SerializeField, Tooltip("How far the cast start is away from the player collider to check for a nearby ground")] private float groundCheckDistance = 0.2f;
    [SerializeField, Tooltip("How far the box cast is being elongated to check for a nearby wall")] private Vector2 wallCheckSize = new Vector2(0.2f, 0.8f);
    [SerializeField, Tooltip("Box collider for collision with objects")] private BoxCollider2D surfaceCollider;
    [SerializeField, Tooltip("Circle collider without collision for wall check")] private CircleCollider2D wallTrigger;

    private Rigidbody2D body;

    private SoulManager soulManager;

    private float _horizontalInput;

    private bool _isGrounded;
    public bool Grounded => _isGrounded;

    // SYSTEMVARIABLEN
    private bool showcaseDoubleJump = false;
    private bool inputLocked = false;

    // COMPONENTS
    // Player Movement Components
    private PlayerRunning movement;
    private PlayerJump jump;
    private PlayerWallActions wallActions;
    private PlayerClimb climb;
    // Animator component
    private Animator animator;

    private PlayerState playerState;

    
        
    
    private void Awake()
    {
        soulManager = GameObject.Find("GameManager").GetComponent<SoulManager>();
        body = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerRunning>();
        jump = GetComponent<PlayerJump>();
        wallActions = GetComponent<PlayerWallActions>();
        climb = GetComponent<PlayerClimb>();
        climb.Body = body;

        if (surfaceCollider == null)
        {
            // 1. Erst am eigenen GameObject suchen
            surfaceCollider = GetComponent<BoxCollider2D>();

            // 2. Falls nicht gefunden, in Children suchen
            if (surfaceCollider == null)
            {
                surfaceCollider = GetComponentInChildren<BoxCollider2D>();

                if (surfaceCollider != null)
                {
                    Debug.Log($"BoxCollider2D gefunden in Child: {surfaceCollider.gameObject.name}");
                }
            }
        }

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

        if (animator == null)
        {
            animator = GetComponent<Animator>();

            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();

                if (animator != null)
                    Debug.Log($"Animator gefunden in Child: {animator.gameObject.name}");
            }
        }

        if (animator == null)
            Debug.LogError("Animator nicht gefunden!");
        Debug.Log("Player Controller Awake Done.");
    }

    private void Start()
    {
        if (soulManager.HasSoul("rabbitSoul"))
            jump.CanDoubleJump = true;

        playerState = PlayerState.DEFAULT;
        Debug.Log("Player Controller Start Done.");
    }

    private void Update()
    {
        if (inputLocked)
        {
            SetInputLocked(true);
            return;
        }

        // DOUBLE-JUMP DEMO
        if (showcaseDoubleJump && _isGrounded)
        {
            showcaseDoubleJump = false;
            StartCoroutine(PlayDoubleJumpShowcase());
        }

        if (Input.GetKeyDown(KeyCode.A) ^ Input.GetKeyDown(KeyCode.D) && _isGrounded)
            playerState = PlayerState.RUNNING;

        if (Input.GetKeyUp(KeyCode.Space) && body.linearVelocity.y > 0)
            playerState = PlayerState.FALLING;

        // Prioritize walljump over normal jump
        if (!wallActions.HandleWallActions(ref body, ref wallLayer, _isGrounded))
        {
            // If no walljump was executed, test normal jump action
            if (jump.HandleJump(_isGrounded))
            {
                // If normal jump was successfully executed, reset wall jump air control duration for a normal movement
                wallActions.ResetWallJumpAirControlDuration();
            }
        }
        else jump.ResetDoubleJumps(); // If walljump was executed, reset the double jumps

        // ANIMATOR EDITS
        if ((Input.GetKey(KeyCode.A) ^ Input.GetKey(KeyCode.D)) && _isGrounded)
            animator.SetBool("IsWalking", true);
        else
            animator.SetBool("IsWalking", false);
    }

    private void FixedUpdate()
    {
        if (inputLocked)
        {
            SetInputLocked(true);
            return;
        }

        float horizontalInput = _horizontalInput = Input.GetAxisRaw("Horizontal");
        _isGrounded = IsGrounded();

        // PLAYER-SPRITE DIRECTION
        if (horizontalInput != 0)
        {
            float dir = horizontalInput > 0 ? 1 : -1;
            transform.localScale = new Vector3(dir * 0.7f, 0.7f, transform.localScale.z);
        }

        if (wallActions.T > 0)
            movement.ApplyWalljumpMovement(ref body, horizontalInput, wallActions.T);
        else movement.ApplyNormalMovement(ref body, horizontalInput);

        wallActions.HandleFixedActions(ref body);

        switch (playerState)
        {
            case PlayerState.DEFAULT:
                animator.SetBool("IsWalking", false);

                if (body.linearVelocity.y < 0)
                    playerState = PlayerState.FALLING;
                //Debug.Log("DEFAULT");
                break;
            case PlayerState.RUNNING:
                animator.SetBool("IsWalking", true);

                if (body.linearVelocity.y < 0)
                    playerState = PlayerState.FALLING;
                //Debug.Log("RUNNING");
                break;
            case PlayerState.JUMPING:

                if (body.linearVelocity.y < 0)
                    playerState = PlayerState.FALLING;
                //Debug.Log("JUMPING");
                break;
            case PlayerState.FALLING:
                //Debug.Log("FALLING");
                break;
            case PlayerState.WALLSLIDING:
                //Debug.Log("WALL SLIDING");
                break;
            case PlayerState.WALLJUMPING:
                //Debug.Log("WALLJUMPING");
                break;
            case PlayerState.CLIMBING:
                //Debug.Log("CLIMBING");
                break;
            default:
                //Debug.Log("DEFAULT");
                break;
        }
    }

    private bool IsGrounded()
    {
        float extraHeight = 0.05f;

        RaycastHit2D hit = Physics2D.BoxCast(
            surfaceCollider.bounds.center,
            surfaceCollider.bounds.size,
            0f,
            Vector2.down,
            extraHeight,
            groundLayer
        );

        return hit.collider != null || climb.CanJump;
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

    public void OnSoulCollected(SoulData soul)
    {
        Debug.Log($"Collected Soul. Soul null status: {soul != null}. Soul ID: {soul.soulID}");
        if (soul == null) return;

        if (soul.soulID == "rabbitSoul")
        {
            jump.CanDoubleJump = true;
            showcaseDoubleJump = true;
        }
    }

    public IEnumerator PlayDoubleJumpShowcase()
    {
        inputLocked = true;

        while (!_isGrounded) yield return null;

        yield return new WaitForSeconds(0.3f);
        jump.Jump();
        yield return new WaitForSeconds(0.3f);

        jump.ResetDoubleJumps();
        jump.Jump();

        yield return new WaitForSeconds(2f);
        inputLocked = false;
    }
}