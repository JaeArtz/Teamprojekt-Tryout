using UnityEngine;

public class PlayerRoll : MonoBehaviour
{
    private Rigidbody2D body;
    public Rigidbody2D Body { set { body = value; } }

    [SerializeField, Tooltip("Cooldown gets started when player exits roll. Next roll is only possible after cooldown has ended.")]
    private float rollCooldown;
    private float rollCooldownTimer;

    [SerializeField, Tooltip("How long the player can stay in roll at max.")]
    private float maxRollDuration;
    private float rollDurationTimer;

    [SerializeField, Tooltip("How much the player can decelerate while rolling.")]
    private float brakeForce;
    public float BrakeForce { get; private set; }

    [SerializeField, Tooltip("The multiplier being applied to the normal movement speed while having the speed boost, gained when enter rolling.")]
    private float speedBoost;
    public float SpeedBoost => speedBoost;

    private bool isHoldingR;
    private bool releasedR = true;
    private bool isRolling;
    public bool IsRolling => isRolling;

    private bool isJumping;
    public bool applyBoostedSpeed { get; private set; }
    public bool ApplyBoostedSpeed => applyBoostedSpeed;

    private bool isGrounded;
    public bool IsGrounded { set { isGrounded = value; } }

    private bool canRoll;
    public bool CanRoll {set{ canRoll = value; }}


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canRoll && releasedR && Input.GetKeyDown(KeyCode.R) && rollCooldownTimer <= 0 && rollDurationTimer <= 0)
        {
            isHoldingR = true;
            releasedR = false;
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            isHoldingR = false;
            releasedR = true;
        }
    }

    public void StopRoll()
    {
        if (!isRolling) return;
        isRolling = false;
        rollCooldownTimer = rollCooldown;
    }

    public void StopBoostSpeed()
    {
        applyBoostedSpeed = false;
    }

    private void FixedUpdate()
    {
        if (!body)
            return;

        if (rollCooldownTimer > 0)
            rollCooldownTimer -= Time.fixedDeltaTime;

        if (rollDurationTimer > 0)
            rollDurationTimer -= Time.fixedDeltaTime;

        if (body.linearVelocityY > 0.1f)
            isJumping = true;

        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (canRoll && isHoldingR && !isRolling && horizontalInput != 0 && rollCooldownTimer <= 0)
        {
            isRolling = true;
            isHoldingR = false;
            applyBoostedSpeed = true;
            rollDurationTimer = maxRollDuration;
            BrakeForce = brakeForce;

            if (Mathf.Abs(body.linearVelocityX) != horizontalInput)
                body.linearVelocityX = horizontalInput;
        }

        if (isJumping && body.linearVelocityY <= 0.1f && isGrounded)
        {
            isJumping = false;
            StopBoostSpeed();
            StopRoll();
        }

        if (isRolling && (rollDurationTimer <= 0))
            StopRoll();

        if (isRolling && Mathf.Abs(body.linearVelocityX) < 0.1f)
        {
            StopBoostSpeed();
            StopRoll();
        }

        if (applyBoostedSpeed && rollDurationTimer <= 0)
        {
            StopBoostSpeed();
        }

    }
}
