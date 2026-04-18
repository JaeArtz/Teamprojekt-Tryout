using UnityEngine;

public class PlayerGlide : MonoBehaviour
{
    private Rigidbody2D body;
    public Rigidbody2D Body { set { body = value; } }
    private PlayerRoll roll;
    public PlayerRoll Roll { set { roll = value; } }
    public bool IsGrounded { get; set; }
    public bool IsGliding { get; private set; }

    [SerializeField, Tooltip("The gliding velocity at the start.")]
    private float startingGlidingVelocityY = -1;
    
    [SerializeField, Tooltip("The gliding velocity after the glide speed transition time has passed.")]
    private float endingGlidingVelocityY = -20;
    
    [SerializeField, Tooltip("The time needed to go from startingGlideVelocityY to endingGlidingVelocityY.")]
    private float glideSpeedTransitionDuration = 5;
    private float glideSpeedTransitionTimer;

    private float currentGlidingVelocityY;

    [SerializeField, Tooltip("The time needed to go from startingGlideVelocityY to endingGlidingVelocityY.")]
    private float glideAbortionPenalty = 0.25f;


    private bool isHoldingSpace;
    public bool canGlide;

    private bool isGlideUnlocked; // Permanente Freischaltung durch Seele
    public bool IsGlideUnlocked { set { isGlideUnlocked = value; } }

    private bool canGlideInAir; // Darf man in diesem spezifischen Sprung gleiten?

    void Awake()
    {
        isGlideUnlocked = false; // Standardmäßig gesperrt
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        glideSpeedTransitionTimer = glideSpeedTransitionDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGlideUnlocked) return; // Wenn nicht freigeschaltet, mache gar nichts

        isHoldingSpace = Input.GetKey(KeyCode.Space);

        if (Input.GetKeyUp(KeyCode.Space) && IsGliding && glideSpeedTransitionTimer > 0 && body.linearVelocityY < currentGlidingVelocityY)
            glideSpeedTransitionTimer = Mathf.Max(0, glideSpeedTransitionTimer - glideAbortionPenalty);

        // Nur wenn freigeschaltet, darf man beim Drücken von Space das Gleiten für diesen Sprung aktivieren
        if(Input.GetKeyDown(KeyCode.Space))
            canGlideInAir = true; 
    }

    private void FixedUpdate()
    {
        if (!body) return;

        UpdateGlidingStatus();
        float t = 1 - glideSpeedTransitionTimer / glideSpeedTransitionDuration;
        currentGlidingVelocityY = Mathf.Lerp(startingGlidingVelocityY, endingGlidingVelocityY, t);
        if (IsGliding)
        {
            if (glideSpeedTransitionTimer > 0)
                glideSpeedTransitionTimer -= Time.fixedDeltaTime;

            body.linearVelocityY = currentGlidingVelocityY;
        }
        if (IsGrounded) ResetGlideSpeedTransitionTimer();
    }

    private void UpdateGlidingStatus()
    {
        if (IsGrounded)
        {
            canGlideInAir = body.linearVelocityY > 1f; // Reset, wenn man landet
            IsGliding = false;
            return;
        }

        // Wenn die Seele nicht da ist ODER man die Taste für diesen Sprung nicht gedrückt hat
        if (!isGlideUnlocked || !canGlideInAir || !isHoldingSpace)
        {
            IsGliding = false;
            return;
        }

        if (roll && roll.IsRolling)
        {
            IsGliding = false;
            return;
        }

        if (body.linearVelocityY > currentGlidingVelocityY)
        {
            IsGliding = false;
        }
        else
        {
            IsGliding = true; 
        }
    }

    public void ResetGlideSpeedTransitionTimer()
    {
        glideSpeedTransitionTimer = glideSpeedTransitionDuration;
    }
}
