using System.Linq;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [SerializeField, Tooltip("Maximum vertical velocity of player")]
    private float playerMaxVelocityY = 20f;
    public float MaxVelocityY { get => playerMaxVelocityY; set => playerMaxVelocityY = value; }

    [SerializeField, Tooltip("Jump abort force is being applied when the jump button is being released before the player's max jump height was reached")]
    private float playerEarlyJumpAbortForceY = 0.5f;

    [Header("Coyote Time")]
    [SerializeField, Tooltip("How long the player can still perform wall-jump when exiting wall")] private float groundCoyoteTime = 0.1f;
    private float groundCoyoteTimer;

    [Header("Multi Jump")]
    [SerializeField, Tooltip("How many times the player can jump in mid-air")] private int extraJumps = 1;
    private int jumpCounter;

    private RandomAudioPlayer audioPlayer;

    private Rigidbody2D body;
    private bool canDoubleJump = false;
    
    public bool CanDoubleJump
    {
        set
        {
            canDoubleJump = value;
        }
    }

    public float VerticalVelocity
    {
        get => body.linearVelocity.y;
        set
        {
            // only sets y-value, in a new vector, x-value stays the same
            body.linearVelocity = new Vector2(body.linearVelocity.x, value);

            // this should ensure that player has an extra jump in air, after bouncing up
            if (value > 0)
            {
                jumpCounter = canDoubleJump ? extraJumps : 0;
                groundCoyoteTimer = 0;
            }
        }
    }

    public float remoteAccessToGroundCoyoteCounter { get => groundCoyoteTimer; set => groundCoyoteTimer = value; }

    public int JumpCounter => jumpCounter;

    private bool _isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        audioPlayer = GetComponents<RandomAudioPlayer>().FirstOrDefault(component => component.Name.Equals("Jump"));

        if (!audioPlayer) Debug.LogError(@"Random Audio Player with name ""Jump"" could not be found!");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_isGrounded && groundCoyoteTimer > 0)
            groundCoyoteTimer -= Time.fixedDeltaTime;
    }

    public void ResetDoubleJumps()
    {
        jumpCounter = extraJumps;
    }

    public bool HandleJump(bool isGrounded)
    {
        _isGrounded = isGrounded;
        if (isGrounded)
        {
            groundCoyoteTimer = groundCoyoteTime;
            jumpCounter = extraJumps;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            if (isGrounded || groundCoyoteTimer > 0)
            {
                GroundJump();
                audioPlayer.PlayRandomSound();
                return true;
            }
            else if (canDoubleJump && jumpCounter > 0)
            {
                DoubleJump();
                audioPlayer.PlayRandomSound();
                return true;
            }

        if (Input.GetKeyUp(KeyCode.Space) && body.linearVelocity.y > 0)
            body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y * playerEarlyJumpAbortForceY);

        return false;
    }

    public void GroundJump()
    {
        Jump();
        jumpCounter = extraJumps;
    }

    public void DoubleJump()
    {
        if (groundCoyoteTimer <= 0)
            if (canDoubleJump && jumpCounter > 0)
                jumpCounter--;
        Jump();
    }

    public void Jump(float? xVel = null, float? yVel = null)
    {
        body.linearVelocity = new Vector2(xVel == null ? body.linearVelocity.x : (float)xVel, yVel == null ? playerMaxVelocityY : (float)yVel);

        if (groundCoyoteTimer > 0)
            groundCoyoteTimer = 0;
    }
}
