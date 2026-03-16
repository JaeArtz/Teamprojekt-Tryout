using UnityEngine;

public class PlayerRunning : MonoBehaviour
{
    [SerializeField, Tooltip("Maximum horizontal velocity of player")] private float playerMaxVelocityX = 10f;
    [SerializeField, Tooltip("Accelerates player to max movement speed. Acceleration speed is applied when player movement direction equals keyboard input direction")]
    private float playerAccelerationX = 5f;
    public float MaxVelocityX { get => playerMaxVelocityX; set => playerMaxVelocityX = value; }

    [SerializeField, Tooltip("Decelerates player to movement speed 0. Deceleration speed is applied when player movement direction is different to keyboard input direction")]
    private float playerDecelerationX = 15f;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void ApplyNormalMovement(ref Rigidbody2D body, float horizontalInput)
    {
        Move(ref body, horizontalInput);
    }

    public void ApplyWalljumpMovement(ref Rigidbody2D body, float horizontalInput, float t)
    {
        float manipulator = Mathf.Lerp(1f, 0f, t);
        Move(ref body, horizontalInput, accelerationRateManipulator: manipulator);
    }

    private void Move(ref Rigidbody2D body, float horizontalInput, float targetSpeedManipulator = 1, float accelerationRateManipulator = 1)
    {
        float targetSpeed = horizontalInput * playerMaxVelocityX * targetSpeedManipulator;
        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? playerAccelerationX * accelerationRateManipulator : playerDecelerationX * accelerationRateManipulator;
        float speedDiff = targetSpeed - body.linearVelocity.x;
        float movement = speedDiff * accelRate * Time.fixedDeltaTime;

        body.linearVelocity = new Vector2(
            Mathf.Clamp(body.linearVelocity.x + movement, -playerMaxVelocityX, playerMaxVelocityX),
            body.linearVelocity.y
        );
    }
}
