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

    private bool isHoldingR;
    private bool releasedR = true;
    private bool isRolling;
    public bool IsRolling => isRolling;

    private bool isJumping;
    public bool applyBoostedSpeed { get; private set; }
    public bool ApplyBoostedSpeed => applyBoostedSpeed;

    private bool isGrounded;
    public bool IsGrounded { set { isGrounded = value; } }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (releasedR && Input.GetKeyDown(KeyCode.R) && /*body.linearVelocityY <= 5 &&*/ rollCooldownTimer <= 0 && rollDurationTimer <= 0)
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

        if (isJumping && body.linearVelocityY <= 0.1f && isGrounded)
        {
            isJumping = false;
            applyBoostedSpeed = false;
        }

        if (isHoldingR && !isRolling && /*body.linearVelocityY <= 5 &&*/ Mathf.Abs(body.linearVelocityX) > 0.1f && rollCooldownTimer <= 0)
        {
            isRolling = true;
            isHoldingR = false;
            applyBoostedSpeed = true;
            rollDurationTimer = maxRollDuration;
            Debug.Log("ROLL!");
        }

        if (isRolling && (rollDurationTimer <= 0 || body.linearVelocityY > 0.1f))
        {
            isRolling = false;
            rollCooldownTimer = rollCooldown;
            Debug.Log("STOP ROLL!");
        }

        if (applyBoostedSpeed && rollDurationTimer <= 0)
            applyBoostedSpeed = false;
    }
}
