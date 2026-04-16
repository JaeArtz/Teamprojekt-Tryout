using System;
using TMPro;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

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
    private PlayerRoll roll;
    private PlayerGlide glide;
    // Animator component
    private Animator animator;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerRunning>();
        jump = GetComponent<PlayerJump>();
        wallActions = GetComponent<PlayerWallActions>();
        climb = GetComponent<PlayerClimb>();
        roll = GetComponent<PlayerRoll>();
        glide = GetComponent<PlayerGlide>();
        movement.Body = body;
        wallActions.Body = body;
        climb.Body = body;
        roll.Body = body;
        glide.Body = body;

        climb.Roll = roll;
        glide.Roll = roll;

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
        if (SoulManager.Instance != null)
        {
            // BRabbit Soul
            if (SoulManager.Instance.HasSoul("rabbitSoul"))
                jump.CanDoubleJump = true;

            // Armadillo Soul beim Start prüfen
            if (SoulManager.Instance.HasSoul("armadilloSoul"))
                roll.CanRoll = true;

            if(SoulManager.Instance.HasSoul("birdSoul"))
                glide.IsGlideUnlocked = true;
        }

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

        // Prioritize walljump over normal jump
        if (!wallActions.HandleWallActions(ref wallLayer, _isGrounded))
        {
            // If no walljump was executed, test normal jump action
            if (jump.HandleJump(_isGrounded || climb.CanJump))
            {
                // If normal jump was successfully executed, reset wall jump air control duration for a normal movement
                wallActions.ResetWallJumpAirControlDuration();
                roll.StopRoll();
            }
        }
        else jump.ResetDoubleJumps(); // If walljump was executed, reset the double jumps
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
        roll.IsGrounded = _isGrounded;
        glide.IsGrounded = _isGrounded;

        // PLAYER-SPRITE DIRECTION
        if (Mathf.Abs(body.linearVelocityX) > 0.1f)
        {
            float dir = body.linearVelocityX > 0 ? 1 : -1;
            transform.localScale = new Vector3(dir * 0.7f, 0.7f, transform.localScale.z);
        }

        //roll.CanRoll = wallActions.CanRoll;

        if (!roll.IsRolling && roll.ApplyBoostedSpeed && !_isGrounded && Mathf.Sign(body.linearVelocityX) != Input.GetAxisRaw("Horizontal"))
            roll.StopBoostSpeed();

        if (roll.ApplyBoostedSpeed)
            movement.ApplyRollMovement(horizontalInput, roll.SpeedBoost, roll.BrakeForce);
        else if (wallActions.T > 0)
            movement.ApplyWalljumpMovement(horizontalInput, wallActions.T);
        else movement.ApplyNormalMovement(horizontalInput);

        wallActions.HandleFixedActions();
    }

    private void LateUpdate()
    {
        // ANIMATOR EDITS
        animator.SetBool("IsWalking", (Input.GetKey(KeyCode.A) ^ Input.GetKey(KeyCode.D)) && _isGrounded);
        animator.SetFloat("xVel", body.linearVelocityX);
        animator.SetFloat("yVel", body.linearVelocityY);
        animator.SetBool("IsClimbing", climb.IsClimbing);
        animator.SetBool("IsRolling", roll.IsRolling);
    }

    private bool IsGrounded()
    {
        float extraHeight = 0.3f;

        RaycastHit2D hit = Physics2D.BoxCast(
            surfaceCollider.bounds.center,
            surfaceCollider.bounds.size,
            0f,
            Vector2.down,
            extraHeight,
            groundLayer
        );

        movement.Hit = hit;

        return hit.collider != null;
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

        if (soul.soulID == "armadilloSoul")
        {
            roll.CanRoll = true;
        }

        if (soul.soulID == "birdSoul")
        {
            glide.IsGlideUnlocked = true;
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